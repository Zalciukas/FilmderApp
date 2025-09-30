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

        var user = await _context.Users.FindAsync(userId);
        if (user == null) return Unauthorized();

        var group = new Group
        {
            Name = dto.Name,
            OwnerId = userId
        };
        
        group.Members.Add(user);

        // Add to group by email
        if (dto.FriendEmails != null && dto.FriendEmails.Any())
        {
            var friends = await _context.Users
                .Where(u => dto.FriendEmails.Contains(u.Email))
                .ToListAsync();

            foreach (var friend in friends)
            {
                if (!group.Members.Any(m => m.Id == friend.Id))
                {
                    group.Members.Add(friend);
                }
            }
        }

        _context.Groups.Add(group);
        await _context.SaveChangesAsync();

        return Ok(new { group.Id, group.Name, group.OwnerId });
    }


    [HttpGet("mine")]
    public async Task<IActionResult> GetMyGroups()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        var groups = await _context.Groups
            .Where(g => g.Members.Any(m => m.Id == userId))
            .Select(g => new { g.Id, g.Name, g.OwnerId })
            .ToListAsync();

        return Ok(groups);
    }
}