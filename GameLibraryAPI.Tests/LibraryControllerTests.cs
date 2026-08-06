using System.Security.Claims;
using GameLibraryAPI.Controllers;
using GameLibraryAPI.DTOs.Game;
using GameLibraryAPI.Helpers;
using GameLibraryAPI.Interfaces;
using GameLibraryAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace GameLibraryAPI.Tests
{
    public class LibraryControllerTests
    {
        //                //
        // Helper methods //
        //                //
        private void SetupControllerUser(LibraryController controller, string username)
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
        public async Task GetUserLibrary_ShouldReturnNotFound_WhenUserDoesNotExist()
        {
            var mockLibraryRepo = new Mock<ILibraryRepository>();
            var mockLogger = new Mock<ILogger<LibraryController>>();
            var mockUserManager = TestHelpers.MockUserManager();

            mockUserManager.Setup(m => m.FindByNameAsync("TestUser")).ReturnsAsync((AppUser)null!);

            var controller = new LibraryController(mockLibraryRepo.Object, mockUserManager.Object, null!, mockLogger.Object);

            var result = await controller.GetUserLibrary("TestUser", new LibraryQueryObject());

            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task GetUserLibrary_ShouldReturnOk_WhenSuccess()
        {
            var mockLibraryRepo = new Mock<ILibraryRepository>();
            var mockLogger = new Mock<ILogger<LibraryController>>();
            var mockUserManager = TestHelpers.MockUserManager();

            var fakeUser = new AppUser { Id = "user-1", UserName = "TestUser"};

            mockUserManager.Setup(m => m.FindByNameAsync("TestUser")).ReturnsAsync(fakeUser);
            mockLibraryRepo.Setup(l => l.GetUserLibraryAsync(fakeUser.Id, It.IsAny<LibraryQueryObject>())).ReturnsAsync(new List<GameDto>());

            var controller = new LibraryController(mockLibraryRepo.Object, mockUserManager.Object, null!, mockLogger.Object);

            var result = await controller.GetUserLibrary("TestUser", new LibraryQueryObject());

            Assert.IsType<OkObjectResult>(result);
        }

        //              //
        // Create tests //
        //              //

        [Fact]
        public async Task AddGameToLibrary_ShouldReturnNotFound_WhenGameDoesNotExist()
        {
            var mockLibraryRepo = new Mock<ILibraryRepository>();
            var mockGameRepo = new Mock<IGameRepository>();
            var mockLogger = new Mock<ILogger<LibraryController>>();
            var mockUserManager = TestHelpers.MockUserManager();

            var fakeUser = new AppUser { Id = "user-1", UserName = "TestUser"};

            mockUserManager.Setup(m => m.FindByNameAsync("TestUser")).ReturnsAsync(fakeUser);
            mockGameRepo.Setup(g => g.GameExistsAsync(1)).ReturnsAsync(false);

            var controller = new LibraryController(mockLibraryRepo.Object, mockUserManager.Object, mockGameRepo.Object, mockLogger.Object);
            SetupControllerUser(controller, "TestUser");

            var result = await controller.AddGameToLibrary(1);

            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task AddGameToLibrary_ShouldReturnBadRequest_WhenUserAlreadyOwnsGame()
        {
            var mockLibraryRepo = new Mock<ILibraryRepository>();
            var mockGameRepo = new Mock<IGameRepository>();
            var mockLogger = new Mock<ILogger<LibraryController>>();
            var mockUserManager = TestHelpers.MockUserManager();

            var fakeUser = new AppUser { Id = "user-1", UserName = "TestUser"};

            mockUserManager.Setup(m => m.FindByNameAsync("TestUser")).ReturnsAsync(fakeUser);
            mockGameRepo.Setup(g => g.GameExistsAsync(1)).ReturnsAsync(true);
            mockLibraryRepo.Setup(l => l.UserOwnsGameAsync(fakeUser.Id, 1)).ReturnsAsync(true);

            var controller = new LibraryController(mockLibraryRepo.Object, mockUserManager.Object, mockGameRepo.Object, mockLogger.Object);
            SetupControllerUser(controller, "TestUser");

            var result = await controller.AddGameToLibrary(1);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task AddGameToLibrary_ShouldReturnOk_WhenSuccess()
        {
            var mockLibraryRepo = new Mock<ILibraryRepository>();
            var mockGameRepo = new Mock<IGameRepository>();
            var mockLogger = new Mock<ILogger<LibraryController>>();
            var mockUserManager = TestHelpers.MockUserManager();

            var fakeUser = new AppUser { Id = "user-1", UserName = "TestUser"};

            mockUserManager.Setup(m => m.FindByNameAsync("TestUser")).ReturnsAsync(fakeUser);
            mockGameRepo.Setup(g => g.GameExistsAsync(1)).ReturnsAsync(true);
            mockLibraryRepo.Setup(l => l.UserOwnsGameAsync(fakeUser.Id, 1)).ReturnsAsync(false);

            var controller = new LibraryController(mockLibraryRepo.Object, mockUserManager.Object, mockGameRepo.Object, mockLogger.Object);
            SetupControllerUser(controller, "TestUser");

            var result = await controller.AddGameToLibrary(1);

            Assert.IsType<OkObjectResult>(result);
        }

        //              //
        // Delete tests //
        //              //

        [Fact]
        public async Task RemoveGameFromLibrary_ShouldReturnBadRequest_WhenGameIsNotInUserLibrary()
        {
            var mockLibraryRepo = new Mock<ILibraryRepository>();
            var mockLogger = new Mock<ILogger<LibraryController>>();
            var mockUserManager = TestHelpers.MockUserManager();

            var fakeUser = new AppUser { Id = "user-1", UserName = "TestUser"};

            mockUserManager.Setup(m => m.FindByNameAsync("TestUser")).ReturnsAsync(fakeUser);
            mockLibraryRepo.Setup(l => l.UserOwnsGameAsync(fakeUser.Id, 1)).ReturnsAsync(false);

            var controller = new LibraryController(mockLibraryRepo.Object, mockUserManager.Object, null!, mockLogger.Object);
            SetupControllerUser(controller, "TestUser");

            var result = await controller.RemoveGameFromLibrary(1);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task RemoveGameFromLibrary_ShouldReturnOk_WhenSuccess()
        {
            var mockLibraryRepo = new Mock<ILibraryRepository>();
            var mockLogger = new Mock<ILogger<LibraryController>>();
            var mockUserManager = TestHelpers.MockUserManager();

            var fakeUser = new AppUser { Id = "user-1", UserName = "TestUser"};

            mockUserManager.Setup(m => m.FindByNameAsync("TestUser")).ReturnsAsync(fakeUser);
            mockLibraryRepo.Setup(l => l.UserOwnsGameAsync(fakeUser.Id, 1)).ReturnsAsync(true);

            var controller = new LibraryController(mockLibraryRepo.Object, mockUserManager.Object, null!, mockLogger.Object);
            SetupControllerUser(controller, "TestUser");

            var result = await controller.RemoveGameFromLibrary(1);

            Assert.IsType<OkObjectResult>(result);
        }
    }
}