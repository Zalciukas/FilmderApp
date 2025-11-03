using Filmder.DTOs;
using Filmder.Models;
using Filmder.Repositories;

namespace Filmder.Services;

public class GroupService(IGroupRepository groups) : IGroupService
{
    public async Task<List<object>> CreateGroupAsync(string currentUserId, CreateGroupDto dto)
    {
        var user = await groups.FindUserByIdAsync(currentUserId);
        if (user == null) throw new UnauthorizedAccessException();

        var group = new Group
        {
            Name = dto.Name,
            OwnerId = currentUserId
        };

        await groups.AddGroupAsync(group);
        await groups.SaveChangesAsync();

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
            var friends = await groups.FindUsersByEmailsAsync(dto.FriendEmails);
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

        await groups.AddGroupMembersAsync(groupMembers);
        await groups.SaveChangesAsync();

        return await groups.GetUserGroupsProjectionAsync(currentUserId);
    }

    public Task<List<object>> GetMyGroupsAsync(string currentUserId)
    {
        return groups.GetUserGroupsProjectionAsync(currentUserId);
    }
}


