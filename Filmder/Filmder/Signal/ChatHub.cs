using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Filmder.Signal;

[Authorize]
public class ChatHub : Hub
{

    public async Task JoinGroup(int groupId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, groupId.ToString());
    }

    public async Task SendMessageToGroup(int groupId, string message)
    {
        var userName = Context.User.FindFirstValue(ClaimTypes.Name) ?? "Unknown";
        await Clients.Group(groupId.ToString())
            .SendAsync("getMessage", userName, message);
    }
}