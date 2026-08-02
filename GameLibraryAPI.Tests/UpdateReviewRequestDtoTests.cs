using GameLibraryAPI.DTOs.Review;

namespace GameLibraryAPI.Tests
{
    public class UpdateReviewRequestDtoTests
    {
        [Fact]
        public void Rating_ShouldRoundToOneDecimal()
        {
            var dto = new UpdateReviewRequestDto();

            dto.Rating = 7.819;

            Assert.Equal(7.8, dto.Rating);
        }
    }
}