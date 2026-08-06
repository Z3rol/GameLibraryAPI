using System.Security.Claims;
using GameLibraryAPI.Controllers;
using GameLibraryAPI.DTOs.Game;
using GameLibraryAPI.Interfaces;
using GameLibraryAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace GameLibraryAPI.Tests
{
    public class GameControllerTests
    {
        //                //
        // Helper methods //
        //                //
        
        private void SetupControllerUser(GameController controller, string username)
        {
            var claims = new List<Claim> { new Claim(ClaimTypes.Name, username) };
            var identity = new ClaimsIdentity(claims, "TestAuth");

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(identity)
                }
            };
        }

        //           //
        // Get tests //
        //           //

        [Fact]
        public async Task GetById_ShouldReturnNotFound_WhenGameDoesNotExist()
        {
            var mockGameRepo = new Mock<IGameRepository>();
            var mockLogger = new Mock<ILogger<GameController>>();

            mockGameRepo.Setup(g => g.GetByIdAsync(1)).ReturnsAsync((GameDto)null!);

            var controller = new GameController(mockGameRepo.Object, mockLogger.Object);

            var result = await controller.GetById(1);

            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task GetById_ShouldReturnOk_WithCorrectDto()
        {
            var mockGameRepo = new Mock<IGameRepository>();
            var mockLogger = new Mock<ILogger<GameController>>();

            var fakeGameDto = new GameDto { Id = 1, Name = "Elden Ring", Genre = "RPG", AverageRating = 9.4 };

            mockGameRepo.Setup(g => g.GetByIdAsync(1)).ReturnsAsync(fakeGameDto);

            var controller = new GameController(mockGameRepo.Object, mockLogger.Object);

            var result = await controller.GetById(1);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedDto = Assert.IsType<GameDto>(okResult.Value);

            Assert.Equal("Elden Ring", returnedDto.Name);
            Assert.Equal(9.4, returnedDto.AverageRating);
        }
        
        //              //
        // Create tests //
        //              //

        [Fact]
        public async Task Create_ShouldReturnBadRequest_WhenNameIsAlreadyTaken()
        {
            var mockGameRepo = new Mock<IGameRepository>();
            var mockLogger = new Mock<ILogger<GameController>>();

            mockGameRepo.Setup(r => r.GameExistsAsync(It.IsAny<string>())).ReturnsAsync(true);

            var controller = new GameController(mockGameRepo.Object, mockLogger.Object);

            var createDto = new CreateGameRequestDto();

            var result = await controller.Create(createDto);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task Create_ShouldReturnCreatedAtAction_WhenSuccess()
        {
            var mockGameRepo = new Mock<IGameRepository>();
            var mockLogger = new Mock<ILogger<GameController>>();

            mockGameRepo.Setup(r => r.GameExistsAsync(It.IsAny<string>())).ReturnsAsync(false);

            var controller = new GameController(mockGameRepo.Object, mockLogger.Object);
            SetupControllerUser(controller, "TestUser");

            var createDto = new CreateGameRequestDto();

            var result = await controller.Create(createDto);

            Assert.IsType<CreatedAtActionResult>(result);
        }

        //              //
        // Update tests //
        //              //

        [Fact]
        public async Task UpdateDetails_ShouldReturnNotFound_WhenGameNotFound()
        {
            var mockGameRepo = new Mock<IGameRepository>();
            var mockLogger = new Mock<ILogger<GameController>>();

            mockGameRepo.Setup(g => g.GetGameEntityByIdAsync(1)).ReturnsAsync((Game)null!);

            var controller = new GameController(mockGameRepo.Object, mockLogger.Object);

            var updateDto = new UpdateGameDetailsDto();

            var result = await controller.UpdateDetails(1, updateDto);

            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task UpdateDetails_ShouldReturnBadRequest_WhenNewNameIsAlreadyTaken()
        {
            var mockGameRepo = new Mock<IGameRepository>();
            var mockLogger = new Mock<ILogger<GameController>>();

            var fakeGame = new Game { Name = "Old name" };

            mockGameRepo.Setup(g => g.GetGameEntityByIdAsync(1)).ReturnsAsync(fakeGame);
            mockGameRepo.Setup(g => g.GameExistsAsync("New name")).ReturnsAsync(true);

            var controller = new GameController(mockGameRepo.Object, mockLogger.Object);

            var updateDto = new UpdateGameDetailsDto { Name = "New name" };

            var result = await controller.UpdateDetails(1, updateDto);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task UpdateDetails_ShouldNotCheckIfNameIsTaken_WhenNameIsUnchanged()
        {
            var mockGameRepo = new Mock<IGameRepository>();
            var mockLogger = new Mock<ILogger<GameController>>();

            var fakeGame = new Game { Name = "Old name", Genre = "Old genre" };

            mockGameRepo.Setup(g => g.GetGameEntityByIdAsync(1)).ReturnsAsync(fakeGame);

            var controller = new GameController(mockGameRepo.Object, mockLogger.Object);
            SetupControllerUser(controller, "TestUser");

            var updateDto = new UpdateGameDetailsDto { Name = "Old name", Genre = "New genre"};

            var result = await controller.UpdateDetails(1, updateDto);

            Assert.IsType<OkObjectResult>(result);
            mockGameRepo.Verify(g => g.GameExistsAsync(It.IsAny<string>()), Times.Never());
        }

        //              //
        // Delete tests //
        //              //

        [Fact]
        public async Task Delete_ShouldReturnNotFound_WhenGameDoesNotExist()
        {
            var mockGameRepo = new Mock<IGameRepository>();
            var mockLogger = new Mock<ILogger<GameController>>();

            mockGameRepo.Setup(g => g.DeleteAsync(1)).ReturnsAsync((Game)null!);

            var controller = new GameController(mockGameRepo.Object, mockLogger.Object);

            var result = await controller.Delete(1);

            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task Delete_ShouldReturnNoContent_WhenSuccess()
        {
            var mockGameRepo = new Mock<IGameRepository>();
            var mockLogger = new Mock<ILogger<GameController>>();

            mockGameRepo.Setup(g => g.DeleteAsync(1)).ReturnsAsync(new Game { Id = 1 });

            var controller = new GameController(mockGameRepo.Object, mockLogger.Object);
            SetupControllerUser(controller, "TestUser");

            var result = await controller.Delete(1);

            Assert.IsType<NoContentResult>(result);
        }
    }
}