using Filmder.Models;

namespace Filmder.Repositories;

public interface IGroupRepository
{
    Task<AppUser?> FindUserByIdAsync(string userId);
    Task<List<AppUser>> FindUsersByEmailsAsync(IEnumerable<string> emails);
    Task AddGroupAsync(Group group);
    Task AddGroupMembersAsync(IEnumerable<GroupMember> members);
    Task SaveChangesAsync();
    Task<List<object>> GetUserGroupsProjectionAsync(string userId);
}


