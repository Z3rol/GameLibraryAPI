using GameLibraryAPI.DTOs.Review;
using GameLibraryAPI.Models;
using GameLibraryAPI.Repository;

namespace GameLibraryAPI.Tests
{
    public class ReviewRepositoryTests
    {
        [Fact]
        public async Task UpdateReviewAsync_ShouldKeepFieldsUnchanged_WhenPartialUpdate()
        {
            var context = TestHelpers.CreateInMemoryContext();
            var repo = new ReviewRepository(context);

            var review = new Review
            {
                AppUserId = "user-1",
                GameId = 1,
                Title = "Original title",
                Content = "Original content",
                Rating = 5
            };

            context.Reviews.Add(review);
            await context.SaveChangesAsync();

            var updateDto = new UpdateReviewRequestDto { Content = "New content" };

            var result = await repo.UpdateReviewAsync(review, updateDto);

            Assert.Equal("Original title", result.Title);
            Assert.Equal("New content", result.Content);
            Assert.Equal(5, result.Rating);
        }
    }
}