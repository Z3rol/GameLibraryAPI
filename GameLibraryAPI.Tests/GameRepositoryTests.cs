using GameLibraryAPI.Helpers;
using GameLibraryAPI.Models;
using GameLibraryAPI.Repository;

namespace GameLibraryAPI.Tests
{
    public class GameRepositoryTests
    {
        //           //
        // Get tests //
        //           //

        [Fact]
        public async Task GetAllAsync_ShouldFilterByNameContains()
        {
            var context = TestHelpers.CreateInMemoryContext();
            var repo = new GameRepository(context);

            context.Games.Add(new Game {
                Id = 1,
                Name = "Game 1",
                Genre = "Rpg",
                DeveloperName = "Some studio",
                ReleaseDate = new DateOnly(2020, 1, 1)
            });
            context.Games.Add(new Game {
                Id = 2,
                Name = "Game 2",
                Genre = "Rpg",
                DeveloperName = "Some studio",
                ReleaseDate = new DateOnly(2020, 1, 1)
            });

            await context.SaveChangesAsync();

            // "me 1" (not "1" or "Game 1") tests a mid-string substring match,
            // not just a prefix or exact match — proves the repo uses Contains(), not
            // StartsWith() or an exact equality check.
            var result = await repo.GetAllAsync(new GameQueryObject { Name = "me 1" });

            Assert.Single(result);
            Assert.Equal(1, result[0].Id);
        }

        [Fact]
        public async Task GetAllAsync_ShouldFilterByGenreEquals()
        {
            var context = TestHelpers.CreateInMemoryContext();
            var repo = new GameRepository(context);

            context.Games.Add(new Game {
                Id = 1,
                Name = "Game 1",
                Genre = "Rp",
                DeveloperName = "Some studio",
                ReleaseDate = new DateOnly(2020, 1, 1)
            });
            context.Games.Add(new Game {
                Id = 2,
                Name = "Game 2",
                Genre = "Rpg",
                DeveloperName = "Some studio",
                ReleaseDate = new DateOnly(2020, 1, 1)
            });
            context.Games.Add(new Game {
                Id = 3,
                Name = "Game 3",
                Genre = "Rpgs",
                DeveloperName = "Some studio",
                ReleaseDate = new DateOnly(2020, 1, 1)
            });

            await context.SaveChangesAsync();

            var result = await repo.GetAllAsync(new GameQueryObject { Genre = "Rpg" });

            Assert.Single(result);
            Assert.Equal(2, result[0].Id);
        }

        [Fact]
        public async Task GetAllAsync_ShouldFilterByReleasedAfterAndBefore()
        {
            var context = TestHelpers.CreateInMemoryContext();
            var repo = new GameRepository(context);

            context.Games.Add(new Game {
                Id = 1,
                Name = "Game 1",
                Genre = "Rpg",
                DeveloperName = "Some studio",
                ReleaseDate = new DateOnly(2020, 1, 1)
            });
            context.Games.Add(new Game {
                Id = 2,
                Name = "Game 2",
                Genre = "Rpg",
                DeveloperName = "Some studio",
                ReleaseDate = new DateOnly(2023, 1, 1)
            });
            context.Games.Add(new Game {
                Id = 3,
                Name = "Game 3",
                Genre = "Rpg",
                DeveloperName = "Some studio",
                ReleaseDate = new DateOnly(2025, 1, 1)
            });

            await context.SaveChangesAsync();

            var result = await repo.GetAllAsync(new GameQueryObject { ReleasedAfter = new DateOnly(2021, 1, 1), ReleasedBefore = new DateOnly(2024, 1, 1) });

            Assert.Single(result);
            Assert.Equal(2, result[0].Id);
        }

        [Fact]
        public async Task GetAllAsync_ShouldSortByReleaseDate()
        {
            var context = TestHelpers.CreateInMemoryContext();
            var repo = new GameRepository(context);

            context.Games.Add(new Game {
                Id = 1,
                Name = "Game 1",
                Genre = "Rpg",
                DeveloperName = "Some studio",
                ReleaseDate = new DateOnly(2025, 1, 1)
            });
            context.Games.Add(new Game {
                Id = 2,
                Name = "Game 2",
                Genre = "Rpg",
                DeveloperName = "Some studio",
                ReleaseDate = new DateOnly(2020, 1, 1)
            });

            await context.SaveChangesAsync();

            var result = await repo.GetAllAsync(new GameQueryObject { SortBy = "ReleaseDate" });

            Assert.Equal(2, result[0].Id);
            Assert.Equal(1, result[1].Id);
        }

        [Fact]
        public async Task GetAllAsync_ShouldSortByReleaseDateDescending()
        {
            var context = TestHelpers.CreateInMemoryContext();
            var repo = new GameRepository(context);

            context.Games.Add(new Game {
                Id = 1,
                Name = "Game 1",
                Genre = "Rpg",
                DeveloperName = "Some studio",
                ReleaseDate = new DateOnly(2020, 1, 1)
            });
            context.Games.Add(new Game {
                Id = 2,
                Name = "Game 2",
                Genre = "Rpg",
                DeveloperName = "Some studio",
                ReleaseDate = new DateOnly(2025, 1, 1)
            });

            await context.SaveChangesAsync();

            var result = await repo.GetAllAsync(new GameQueryObject { SortBy = "ReleaseDate", IsDescending = true });

            Assert.Equal(2, result[0].Id);
            Assert.Equal(1, result[1].Id);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnZeroAverageRating_WhenGameHasNoReviews()
        {
            var context = TestHelpers.CreateInMemoryContext();
            var repo = new GameRepository(context);

            var game = new Game
            {
                Id = 1,
                Name = "New game",
                Genre = "Rpg",
                DeveloperName = "Some studio",
                ReleaseDate = new DateOnly(2020, 1, 1)
            };

            context.Games.Add(game);
            await context.SaveChangesAsync();

            var result = await repo.GetByIdAsync(1);

            Assert.NotNull(result);
            Assert.Equal(0, result.AverageRating);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnCorrectAverageRating_WhenGameHasMultipleReviews()
        {
            var context = TestHelpers.CreateInMemoryContext();
            var repo = new GameRepository(context);

            var game = new Game
            {
                Id = 1,
                Name = "New game",
                Genre = "Rpg",
                DeveloperName = "Some studio",
                ReleaseDate = new DateOnly(2020, 1, 1)
            };

            context.Games.Add(game);

            context.Reviews.Add(new Review { AppUserId = "user-1", GameId = 1, Title = "Title", Content = "Content", Rating = 10 });
            context.Reviews.Add(new Review { AppUserId = "user-2", GameId = 1, Title = "Title", Content = "Content", Rating = 5 });

            await context.SaveChangesAsync();

            var result = await repo.GetByIdAsync(1);

            Assert.NotNull(result);
            Assert.Equal(7.5, result.AverageRating);
        }
    }
}