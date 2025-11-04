using Filmder.DTOs;
using Filmder.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Filmder.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class GroupController(IGroupService groupService) : ControllerBase
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
