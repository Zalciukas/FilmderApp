using Filmder.Data;
using Filmder.DTOs;
using Filmder.Models;
using Microsoft.EntityFrameworkCore;

namespace Filmder.Services;

public class GroupService(AppDbContext context) : IGroupService
{
    public async Task<List<object>> CreateGroupAsync(string currentUserId, CreateGroupDto dto)
    {
        var user = await context.Users.FindAsync(currentUserId);
        if (user == null) throw new UnauthorizedAccessException();

        var group = new Group
        {
            Name = dto.Name,
            OwnerId = currentUserId
        };

        await context.Groups.AddAsync(group);
        await context.SaveChangesAsync();

        var groupMembers = new List<GroupMember>
        {
            new GroupMember
            {
                UserId = currentUserId,
                GroupId = group.Id,
                JoinedAt = DateTime.UtcNow
            }
        };

        if (dto.FriendEmails != null && dto.FriendEmails.Any())
        {
            var friends = await context.Users
                .Where(u => dto.FriendEmails.Contains(u.Email!))
                .ToListAsync();
            
            foreach (var friend in friends)
            {
                if (friend.Id != currentUserId)
                {
                    groupMembers.Add(new GroupMember
                    {
                        UserId = friend.Id,
                        GroupId = group.Id,
                        JoinedAt = DateTime.UtcNow
                    });
                }
            }
        }

        context.GroupMembers.AddRange(groupMembers);
        await context.SaveChangesAsync();

        return await GetUserGroupsProjectionAsync(currentUserId);
    }

    public async Task<List<object>> GetMyGroupsAsync(string currentUserId)
    {
        return await GetUserGroupsProjectionAsync(currentUserId);
    }

    private Task<List<object>> GetUserGroupsProjectionAsync(string userId)
    {
        return context.Groups
            .Where(g => context.GroupMembers
                .Any(m => m.GroupId == g.Id && m.UserId == userId))
            .Select(g => new { g.Id, g.Name, g.OwnerId } as object)
            .ToListAsync();
    }
}


