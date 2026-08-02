using GameLibraryAPI.DTOs.Game;

namespace GameLibraryAPI.Tests
{
    public class CreateGameRequestDtoTests
    {
        [Fact]
        public void Name_ShouldTrimWhitespace()
        {
            var dto = new CreateGameRequestDto();

            dto.Name = "The witcher 3 ";

            Assert.Equal("The witcher 3", dto.Name);
        }

        [Fact]
        public void Name_ShouldBecomeEmptyString_WhenOnlyWhitespace()
        {
            var dto = new CreateGameRequestDto();

            dto.Name = "   ";

            Assert.Equal("", dto.Name);
        }
    }
}