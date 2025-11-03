using Filmder.DTOs;

namespace Filmder.Services;

public interface IGroupService
{
    Task<List<object>> CreateGroupAsync(string currentUserId, CreateGroupDto dto);
    Task<List<object>> GetMyGroupsAsync(string currentUserId);
}


