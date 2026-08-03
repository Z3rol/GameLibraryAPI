using GameLibraryAPI.Controllers;
using GameLibraryAPI.DTOs.Game;
using GameLibraryAPI.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace GameLibraryAPI.Tests
{
    public class GameControllerTests
    {
        [Fact]
        public async Task Create_ShouldReturnBadRequest_WhenNameIsAlreadyTaken()
        {
            var mockGameRepo = new Mock<IGameRepository>();
            var mockLogger = new Mock<ILogger<GameController>>();

            mockGameRepo.Setup(r => r.GameExistsAsync(It.IsAny<string>())).ReturnsAsync(true);

            var controller = new GameController(
                mockGameRepo.Object, mockLogger.Object);

            var createDto = new CreateGameRequestDto
            {
                Name = "",
                Genre = "",
                DeveloperName = "",
                ReleaseDate = new DateOnly(2025, 10, 5)
            };

            var result = await controller.Create(createDto);

            Assert.IsType<BadRequestObjectResult>(result);
        }
    }
}