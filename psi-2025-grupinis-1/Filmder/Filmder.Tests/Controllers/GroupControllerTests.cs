using Filmder.Controllers;
using Filmder.DTOs;
using Filmder.Models;
using Filmder.Tests.Helpers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;

namespace Filmder.Tests.Controllers;

public class GroupControllerTests
{
    [Fact]
    public async Task CreateGroup_ValidDto_CreatesGroupSuccessfully()
    {
        // Arrange
        var context = TestDbContextFactory.CreateContextWithUsers();
        var controller = new GroupController(context);
        MockHelpers.SetupControllerContext(controller, "user1", "test1@example.com", "test1");

        var createDto = new CreateGroupDto
        {
            Name = "Movie Night Group",
            FriendEmails = new List<string> { "test2@example.com" }
        };

        // Act
        var result = await controller.CreateGroup(createDto);

        // Assert
        var actionResult = result as OkObjectResult;
        actionResult.Should().NotBeNull();
        
        var group = context.Groups.First();
        group.Name.Should().Be("Movie Night Group");
        group.OwnerId.Should().Be("user1");
        
        var members = context.GroupMembers.Where(gm => gm.GroupId == group.Id).ToList();
        members.Should().HaveCount(2); // Owner + 1 friend
    }

    [Fact]
    public async Task CreateGroup_NoFriends_CreatesGroupWithOwnerOnly()
    {
        // Arrange
        var context = TestDbContextFactory.CreateContextWithUsers();
        var controller = new GroupController(context);
        MockHelpers.SetupControllerContext(controller, "user1", "test1@example.com", "test1");

        var createDto = new CreateGroupDto
        {
            Name = "Solo group",
            FriendEmails = new List<string>()
        };
        // Act
        var result = await controller.CreateGroup(createDto);

        // Assert
        var actionResult = result as OkObjectResult;
        actionResult.Should().NotBeNull();
        
        var group = context.Groups.First();
        group.Name.Should().Be("Solo group");
        group.OwnerId.Should().Be("user1");
        
        var members = context.GroupMembers.Where(gm => gm.GroupId == group.Id).ToList();
        members.Should().HaveCount(1);
        members[0].UserId.Should().Be("user1");
    }
    
    [Fact]
    public async Task GetMyGroups_UserInMultipleGroups_ReturnsAllGroups()
    {
        // Arrange
        var context = TestDbContextFactory.CreateContextWithUsers();
        
        var group1 = new Group { Name = "Group 1", OwnerId = "user1" };
        var group2 = new Group { Name = "Group 2", OwnerId = "user2" };
        context.Groups.AddRange(group1, group2);
        await context.SaveChangesAsync();

        context.GroupMembers.AddRange(
            new GroupMember { UserId = "user1", GroupId = group1.Id },
            new GroupMember { UserId = "user1", GroupId = group2.Id }
        );
        await context.SaveChangesAsync();
        
        var controller = new GroupController(context);
        MockHelpers.SetupControllerContext(controller, "user1", "test1@example.com", "test1");

        // Act
        var result = await controller.GetMyGroups();

        // Assert
        var actionResult = result as OkObjectResult;
        actionResult.Should().NotBeNull();
        actionResult.Value.Should().NotBeNull();
        
        // The controller returns an anonymous type, so we need to use reflection or JSON serialization
        var groupsList = actionResult.Value as IEnumerable<object>;
        groupsList.Should().NotBeNull();
        groupsList.Should().HaveCount(2);
    }


    [Fact]
    public async Task GetMyGroups_UserInNoGroups_ReturnsEmptyList()
    {
        // Arrange
        var context = TestDbContextFactory.CreateContextWithUsers();
        var controller = new GroupController(context);
        MockHelpers.SetupControllerContext(controller, "user1", "test1@exmaple.com", "test1");
        
        // Act
        var result = await controller.GetMyGroups();

        // Assert
        var actionResult = result as OkObjectResult;
        actionResult.Should().NotBeNull();
    }
    
    [Fact]
    public async Task CreateGroup_NonExistentFriendEmail_IgnoresFriend()
    {
        // Arrange
        var context = TestDbContextFactory.CreateContextWithUsers();
        var controller = new GroupController(context);
        MockHelpers.SetupControllerContext(controller, "user1", "test1@example.com", "test1");

        var createDto = new CreateGroupDto
        {
            Name = "Test Group",
            FriendEmails = new List<string> { "FAKE@example.com" }
        };

        // Act
        var result = await controller.CreateGroup(createDto);

        // Assert
        var actionResult = result as OkObjectResult;
        actionResult.Should().NotBeNull();
        
        var group = context.Groups.First();
        var members = context.GroupMembers.Where(gm => gm.GroupId == group.Id).ToList();
        members.Should().HaveCount(1); 
    }
}