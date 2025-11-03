using Filmder.Data;
using Filmder.DTOs;
using Filmder.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Filmder.Services;

namespace Filmder.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class GroupController(AppDbContext context, IGroupService groupService) : ControllerBase
{
    [HttpPost("create")]
    public async Task<IActionResult> CreateGroup([FromBody] CreateGroupDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        try
        {
            var groups = await groupService.CreateGroupAsync(userId, dto);
            return Ok(groups);
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized();
        }
    }

    [HttpGet("mine")]
    public async Task<IActionResult> GetMyGroups()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var groups = await groupService.GetMyGroupsAsync(userId);
        return Ok(groups);
    }
}
