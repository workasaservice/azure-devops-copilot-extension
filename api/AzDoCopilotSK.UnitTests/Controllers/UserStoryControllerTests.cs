using Microsoft.AspNetCore.Mvc;
using AzDoCopilotSK.Controllers;
using AzDoCopilotSK.Models;
using AzDoCopilotSK.SK;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Moq;
using Xunit;

namespace AzDoCopilotSK.UnitTests.Controllers
{
public class UserStoryControllerTests
{
    private readonly Mock<Kernel> _mockKernel;
    private readonly Mock<IPromptsFactory> _mockPromptsFactory;
    private readonly Mock<ILogger<UserStoryController>> _mockLogger;
    private readonly Mock<UserStorySkill> _mockUserStorySkill;
    private readonly UserStoryController _controller;

    public UserStoryControllerTests()
    {
        _mockKernel = new Mock<Kernel>();
        _mockPromptsFactory = new Mock<IPromptsFactory>();
        _mockLogger = new Mock<ILogger<UserStoryController>>();
        _mockUserStorySkill = new Mock<UserStorySkill>(_mockKernel.Object, _mockPromptsFactory.Object);

        _controller = new UserStoryController(_mockKernel.Object, _mockPromptsFactory.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task Delete_ReturnsNoContent_WhenUserStoryIsDeleted()
    {
        // Arrange
        var userStoryId = Guid.NewGuid();
        _mockUserStorySkill.Setup(skill => skill.DeleteUserStory(userStoryId)).ReturnsAsync(true);

        // Act
        var result = await _controller.Delete(userStoryId);

        // Assert
        Assert.IsType<NoContentResult>(result);
        _mockLogger.Verify(logger => logger.LogInformation("User story with ID: {Id} successfully deleted.", userStoryId), Times.Once);
    }

    [Fact]
    public async Task Delete_ReturnsNotFound_WhenUserStoryDoesNotExist()
    {
        // Arrange
        var userStoryId = Guid.NewGuid();
        _mockUserStorySkill.Setup(skill => skill.DeleteUserStory(userStoryId)).ReturnsAsync(false);

        // Act
        var result = await _controller.Delete(userStoryId);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal($"User story with ID {userStoryId} not found.", ((dynamic)notFoundResult.Value).Message);
        _mockLogger.Verify(logger => logger.LogWarning("User story with ID: {Id} not found.", userStoryId), Times.Once);
    }

    [Fact]
    public async Task Delete_ReturnsInternalServerError_WhenExceptionIsThrown()
    {
        // Arrange
        var userStoryId = Guid.NewGuid();
        _mockUserStorySkill.Setup(skill => skill.DeleteUserStory(userStoryId)).ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _controller.Delete(userStoryId);

        // Assert
        var statusCodeResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, statusCodeResult.StatusCode);
        Assert.Equal("An error occurred while processing your request.", ((dynamic)statusCodeResult.Value).Message);
        _mockLogger.Verify(logger => logger.LogError(It.IsAny<Exception>(), "An error occurred while deleting user story with ID: {Id}", userStoryId), Times.Once);
    }
}
}
