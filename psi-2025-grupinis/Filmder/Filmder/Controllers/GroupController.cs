using Filmder.Data;
using Filmder.DTOs;
using Filmder.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Filmder.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class GroupController(AppDbContext context) : ControllerBase
{
    [HttpPost("create")]
    public async Task<IActionResult> CreateGroup([FromBody] CreateGroupDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        var user = await context.Users.FindAsync(userId);
        if (user == null) return Unauthorized();

        var group = new Group
        {
            Name = dto.Name,
            OwnerId = userId
        };

        context.Groups.Add(group);
        await context.SaveChangesAsync(); 

        var groupMembers = new List<GroupMember>
        {
            new GroupMember
            {
                UserId = userId,
                GroupId = group.Id,
                JoinedAt = DateTime.UtcNow
            }
        };

        if (dto.FriendEmails != null && dto.FriendEmails.Any())
        {
            var friends = await context.Users
                .Where(u => dto.FriendEmails.Contains(u.Email))
                .ToListAsync();

            foreach (var friend in friends)
            {
                if (friend.Id != userId)
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

        var groups = await context.Groups
            .Where(g => context.GroupMembers
                .Any(m => m.GroupId == g.Id && m.UserId == userId))
            .Select(g => new { g.Id, g.Name, g.OwnerId })
            .ToListAsync();

        return Ok(groups);
    }

    [HttpGet("mine")]
    public async Task<IActionResult> GetMyGroups()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        var groups = await context.Groups
            .Where(g => context.GroupMembers
                .Any(m => m.GroupId == g.Id && m.UserId == userId))
            .Select(g => new { g.Id, g.Name, g.OwnerId })
            .ToListAsync();

        return Ok(groups);
    }
}
