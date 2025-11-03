using Filmder.Data;
using Filmder.Models;
using Microsoft.EntityFrameworkCore;

namespace Filmder.Repositories;

public class GroupRepository(AppDbContext context) : IGroupRepository
{
    public Task<AppUser?> FindUserByIdAsync(string userId)
    {
        return context.Users.FindAsync(userId).AsTask();
    }

    public Task<List<AppUser>> FindUsersByEmailsAsync(IEnumerable<string> emails)
    {
        return context.Users.Where(u => emails.Contains(u.Email!)).ToListAsync();
    }

    public async Task AddGroupAsync(Group group)
    {
        await context.Groups.AddAsync(group);
    }

    public Task AddGroupMembersAsync(IEnumerable<GroupMember> members)
    {
        context.GroupMembers.AddRange(members);
        return Task.CompletedTask;
    }

    public Task SaveChangesAsync()
    {
        return context.SaveChangesAsync();
    }

    public Task<List<object>> GetUserGroupsProjectionAsync(string userId)
    {
        return context.Groups
            .Where(g => context.GroupMembers
                .Any(m => m.GroupId == g.Id && m.UserId == userId))
            .Select(g => new { g.Id, g.Name, g.OwnerId } as object)
            .ToListAsync();
    }
}


