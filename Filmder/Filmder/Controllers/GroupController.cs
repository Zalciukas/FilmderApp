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
public class GroupController : ControllerBase
{
    private readonly AppDbContext _context;

    public GroupController(AppDbContext context)
    {
        _context = context;
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateGroup([FromBody] CreateGroupDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        // Validate creator
        var user = await _context.Users.FindAsync(userId);
        if (user == null) return Unauthorized();

        // Create new group
        var group = new Group
        {
            Name = dto.Name,
            OwnerId = userId
        };

        _context.Groups.Add(group);
        await _context.SaveChangesAsync(); 

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
            var friends = await _context.Users
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

        _context.GroupMembers.AddRange(groupMembers);
        await _context.SaveChangesAsync();

        var groups = await _context.Groups
            .Where(g => _context.GroupMembers
                .Any(m => m.GroupId == g.Id && m.UserId == userId))
            .Select(g => new { g.Id, g.Name, g.OwnerId })
            .ToListAsync();

        return Ok(groups);
    }

    [HttpGet("mine")]
    public async Task<IActionResult> GetMyGroups()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        var groups = await _context.Groups
            .Where(g => _context.GroupMembers
                .Any(m => m.GroupId == g.Id && m.UserId == userId))
            .Select(g => new { g.Id, g.Name, g.OwnerId })
            .ToListAsync();

        return Ok(groups);
    }
}
