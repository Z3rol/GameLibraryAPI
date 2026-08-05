using System.Security.Claims;
using GameLibraryAPI.Controllers;
using GameLibraryAPI.DTOs.Review;
using GameLibraryAPI.Helpers;
using GameLibraryAPI.Interfaces;
using GameLibraryAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace GameLibraryAPI.Tests
{
    public class ReviewControllerTests
    {
        //                //
        // Helper methods //
        //                //
        
        private void SetupControllerUser(ReviewController controller, string username)
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

        //            //
        // Read tests //
        //            //

        [Fact]
        public async Task GetReviewsByGameId_ShouldReturnNotFound_WhenGameDoesNotExist()
        {
            var mockReviewRepo = new Mock<IReviewRepository>();
            var mockGameRepo = new Mock<IGameRepository>();
            var mockLogger = new Mock<ILogger<ReviewController>>();

            mockGameRepo.Setup(r => r.GameExistsAsync(5)).ReturnsAsync(false);

            var controller = new ReviewController(mockReviewRepo.Object, mockGameRepo.Object, null!, mockLogger.Object);

            var result = await controller.GetReviewsByGameId(5, new ReviewQueryObject());

            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task GetReviewsByUsername_ShouldReturnNotFound_WhenUserDoesNotExist()
        {
            var mockReviewRepo = new Mock<IReviewRepository>();
            var mockLogger = new Mock<ILogger<ReviewController>>();
            var mockUserManager = TestHelpers.MockUserManager();

            mockUserManager.Setup(m => m.FindByNameAsync("NotValidUser")).ReturnsAsync((AppUser)null!);

            var controller = new ReviewController(mockReviewRepo.Object, null!, mockUserManager.Object, mockLogger.Object);

            var result = await controller.GetReviewsByUsername("NotValidUser", new ReviewQueryObject());

            Assert.IsType<NotFoundObjectResult>(result);
        }

        //              //
        // Create tests //
        //              //

        [Fact]
        public async Task CreateReview_ShouldReturnNotFound_WhenGameDoesNotExist()
        {
            var mockReviewRepo = new Mock<IReviewRepository>();
            var mockGameRepo = new Mock<IGameRepository>();
            var mockLogger = new Mock<ILogger<ReviewController>>();
            var mockUserManager = TestHelpers.MockUserManager();

            var fakeUser = new AppUser { Id = "user-1", UserName = "TestUser"};

            mockUserManager.Setup(m => m.FindByNameAsync("TestUser")).ReturnsAsync(fakeUser);
            mockGameRepo.Setup(g => g.GameExistsAsync(1)).ReturnsAsync(false);

            var controller = new ReviewController(mockReviewRepo.Object, mockGameRepo.Object, mockUserManager.Object, mockLogger.Object);
            SetupControllerUser(controller, "TestUser");

            var createDto = new CreateReviewRequestDto { GameId = 1 };

            var result = await controller.CreateReview(createDto);

            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task CreateReview_ShouldReturnBadRequest_WhenUserHasReviewedGame()
        {
            var mockReviewRepo = new Mock<IReviewRepository>();
            var mockGameRepo = new Mock<IGameRepository>();
            var mockLogger = new Mock<ILogger<ReviewController>>();
            var mockUserManager = TestHelpers.MockUserManager();

            var fakeUser = new AppUser { Id = "user-1", UserName = "TestUser"};

            mockUserManager.Setup(m => m.FindByNameAsync("TestUser")).ReturnsAsync(fakeUser);
            mockGameRepo.Setup(g => g.GameExistsAsync(1)).ReturnsAsync(true);
            mockReviewRepo.Setup(r => r.UserHasReviewedGameAsync(fakeUser.Id, 1)).ReturnsAsync(true);

            var controller = new ReviewController(mockReviewRepo.Object, mockGameRepo.Object, mockUserManager.Object, mockLogger.Object);
            SetupControllerUser(controller, "TestUser");

            var createDto = new CreateReviewRequestDto { GameId = 1 };

            var result = await controller.CreateReview(createDto);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task CreateReview_ShouldReturnCreatedAtAction_WhenSuccess()
        {
            var mockReviewRepo = new Mock<IReviewRepository>();
            var mockGameRepo = new Mock<IGameRepository>();
            var mockLogger = new Mock<ILogger<ReviewController>>();
            var mockUserManager = TestHelpers.MockUserManager();

            var fakeUser = new AppUser { Id = "user-1", UserName = "TestUser"};

            mockUserManager.Setup(m => m.FindByNameAsync("TestUser")).ReturnsAsync(fakeUser);
            mockGameRepo.Setup(g => g.GameExistsAsync(1)).ReturnsAsync(true);
            mockReviewRepo.Setup(r => r.UserHasReviewedGameAsync(fakeUser.Id, 1)).ReturnsAsync(false);

            var controller = new ReviewController(mockReviewRepo.Object, mockGameRepo.Object, mockUserManager.Object, mockLogger.Object);
            SetupControllerUser(controller, "TestUser");

            var createDto = new CreateReviewRequestDto { GameId = 1 };

            var result = await controller.CreateReview(createDto);

            Assert.IsType<CreatedAtActionResult>(result);
        }

        //              //
        // Update tests //
        //              //

        [Fact]
        public async Task UpdateReview_ShouldReturnBadRequest_WhenAllFieldsAreNull()
        {
            var mockReviewRepo = new Mock<IReviewRepository>();
            var mockLogger = new Mock<ILogger<ReviewController>>();

            var controller = new ReviewController(mockReviewRepo.Object, null!, null!, mockLogger.Object);

            var updateDto = new UpdateReviewRequestDto();

            var result = await controller.UpdateReview(1, updateDto);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task UpdateReview_ShouldReturnOk_WhenAtleastOneFieldIsSet()
        {
            var mockReviewRepo = new Mock<IReviewRepository>();
            var mockLogger = new Mock<ILogger<ReviewController>>();
            var mockUserManager = TestHelpers.MockUserManager();

            var fakeUser = new AppUser { Id = "user-1", UserName = "TestUser"};
            var fakeReview = new Review { Id = 1, AppUserId = "user-1", GameId = 1, Title = "Old", Content = "Old content", Rating = 4};

            mockUserManager.Setup(m => m.FindByNameAsync("TestUser")).ReturnsAsync(fakeUser);
            mockReviewRepo.Setup(r => r.GetReviewByUserAndGameAsync(fakeUser.Id, 1)).ReturnsAsync(fakeReview);
            mockReviewRepo.Setup(r => r.UpdateReviewAsync(fakeReview, It.IsAny<UpdateReviewRequestDto>())).ReturnsAsync(fakeReview);

            var controller = new ReviewController(mockReviewRepo.Object, null!, mockUserManager.Object, mockLogger.Object);
            SetupControllerUser(controller, "TestUser");

            var updateDto = new UpdateReviewRequestDto{ Content = "New content" };

            var result = await controller.UpdateReview(1, updateDto);

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task UpdateReview_ShouldReturnNotFound_WhenUserHasNotReviewedGame()
        {
            var mockReviewRepo = new Mock<IReviewRepository>();
            var mockLogger = new Mock<ILogger<ReviewController>>();
            var mockUserManager = TestHelpers.MockUserManager();

            var fakeUser = new AppUser { Id = "user-1", UserName = "TestUser"};

            mockUserManager.Setup(m => m.FindByNameAsync("TestUser")).ReturnsAsync(fakeUser);
            mockReviewRepo.Setup(r => r.GetReviewByUserAndGameAsync(fakeUser.Id, 1)).ReturnsAsync((Review)null!);

            var controller = new ReviewController(mockReviewRepo.Object, null!, mockUserManager.Object, mockLogger.Object);
            SetupControllerUser(controller, "TestUser");

            var updateDto = new UpdateReviewRequestDto { Content = "New content" };

            var result = await controller.UpdateReview(1, updateDto);

            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task UpdateReview_ShouldReturnRepositoryResultAsDto_WhenSuccess()
        {
            var mockReviewRepo = new Mock<IReviewRepository>();
            var mockLogger = new Mock<ILogger<ReviewController>>();
            var mockUserManager = TestHelpers.MockUserManager();

            var fakeUser = new AppUser { Id = "user-1", UserName = "TestUser"};

            var originalReview = new Review
            {
                Id = 1,
                AppUserId = fakeUser.Id,
                GameId = 1,
                Title = "Original title",
                Content = "Original content",
                Rating = 5
            };

            var expectedReview = new Review
            {
                Id = 1,
                AppUserId = fakeUser.Id,
                GameId = 1,
                Title = "Original title",
                Content = "New content",
                Rating = 5
            };

            mockUserManager.Setup(m => m.FindByNameAsync("TestUser")).ReturnsAsync(fakeUser);
            mockReviewRepo.Setup(r => r.GetReviewByUserAndGameAsync(fakeUser.Id, 1)).ReturnsAsync(originalReview);
            mockReviewRepo.Setup(r => r.UpdateReviewAsync(originalReview, It.IsAny<UpdateReviewRequestDto>())).ReturnsAsync(expectedReview);

            var controller = new ReviewController(mockReviewRepo.Object, null!, mockUserManager.Object, mockLogger.Object);
            SetupControllerUser(controller, "TestUser");

            var updateDto = new UpdateReviewRequestDto { Content = "New content" };

            var result = await controller.UpdateReview(1, updateDto);

            var okResult = Assert.IsType<OkObjectResult>(result);

            var returnedDto = Assert.IsType<ReviewDto>(okResult.Value);

            Assert.Equal("Original title", returnedDto.Title);
            Assert.Equal("New content", returnedDto.Content);
            Assert.Equal(5, returnedDto.Rating);
        }

        //              //
        // Delete tests //
        //              //

        [Fact]
        public async Task DeleteReview_ShouldReturnNotFound_WhenReviewDoesNotExist()
        {
            var mockReviewRepo = new Mock<IReviewRepository>();
            var mockLogger = new Mock<ILogger<ReviewController>>();
            var mockUserManager = TestHelpers.MockUserManager();

            var fakeUser = new AppUser { Id = "user-1", UserName = "TestUser"};

            mockUserManager.Setup(m => m.FindByNameAsync("TestUser")).ReturnsAsync(fakeUser);
            mockReviewRepo.Setup(r => r.GetReviewByUserAndGameAsync(fakeUser.Id, 1)).ReturnsAsync((Review)null!);

            var controller = new ReviewController(mockReviewRepo.Object, null!, mockUserManager.Object, mockLogger.Object);
            SetupControllerUser(controller, "TestUser");

            var result = await controller.DeleteReview(1);

            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task DeleteReview_ShouldReturnNoContent_WhenSuccess()
        {
            var mockReviewRepo = new Mock<IReviewRepository>();
            var mockLogger = new Mock<ILogger<ReviewController>>();
            var mockUserManager = TestHelpers.MockUserManager();

            var fakeUser = new AppUser { Id = "user-1", UserName = "TestUser"};
            var fakeReview = new Review { Id = 1, AppUserId = fakeUser.Id, GameId = 1 };

            mockUserManager.Setup(m => m.FindByNameAsync("TestUser")).ReturnsAsync(fakeUser);
            mockReviewRepo.Setup(r => r.GetReviewByUserAndGameAsync(fakeUser.Id, 1)).ReturnsAsync(fakeReview);

            var controller = new ReviewController(mockReviewRepo.Object, null!, mockUserManager.Object, mockLogger.Object);
            SetupControllerUser(controller, "TestUser");

            var result = await controller.DeleteReview(1);

            Assert.IsType<NoContentResult>(result);
        }
    }
}