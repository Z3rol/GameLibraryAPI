using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Castle.Core.Logging;
using GameLibraryAPI.Controllers;
using GameLibraryAPI.Helpers;
using GameLibraryAPI.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace GameLibraryAPI.Tests
{
    public class ReviewControllerTests
    {
        [Fact]
        public async Task GetReviewsByGameId_ShouldReturnNotFound_WhenGameDoesNotExist()
        {
            var mockReviewRepo = new Mock<IReviewRepository>();
            var mockGameRepo = new Mock<IGameRepository>();
            var mockLogger = new Mock<ILogger<ReviewController>>();

            mockGameRepo.Setup(r => r.GameExistsAsync(5)).ReturnsAsync(false);

            var controller = new ReviewController(
                mockReviewRepo.Object, mockGameRepo.Object, null!, mockLogger.Object);

            var result = await controller.GetReviewsByGameId(5, new ReviewQueryObject());

            Assert.IsType<NotFoundObjectResult>(result);
        }
    }
}