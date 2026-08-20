using GameLibraryAPI.Helpers;
using GameLibraryAPI.Models;
using GameLibraryAPI.Repository;

namespace GameLibraryAPI.Tests
{
    public class LibraryRepositoryTests
    {
        //           //
        // Get tests //
        //           //

        [Fact]
        public async Task GetUserLibraryAsync_ShouldFilterByGameNameContains()
        {
            var context = TestHelpers.CreateInMemoryContext();
            var repo = new LibraryRepository(context);

            context.Games.Add(new Game { Id = 1, Name = "Game 1", Genre = "Rpg", DeveloperName = "Some studio", ReleaseDate = new DateOnly(2020, 1, 1) });
            context.Games.Add(new Game { Id = 2, Name = "Game 2", Genre = "Rpg", DeveloperName = "Some studio", ReleaseDate = new DateOnly(2020, 1, 1) });

            context.UserGames.Add(new UserGame { GameId = 1, AppUserId = "user-1" });
            context.UserGames.Add(new UserGame { GameId = 2, AppUserId = "user-1" });

            await context.SaveChangesAsync();

            // "me 1" (not "1" or "Game 1") tests a mid-string substring match,
            // not just a prefix or exact match — proves the repo uses Contains(), not
            // StartsWith() or an exact equality check.
            var result = await repo.GetUserLibraryAsync("user-1", new LibraryQueryObject { GameName = "me 1" });

            Assert.Single(result);
            Assert.Equal(1, result[0].Id);
        }

        [Fact]
        public async Task GetUserLibraryAsync_ShouldFilterByMinAverageRating()
        {
            var context = TestHelpers.CreateInMemoryContext();
            var repo = new LibraryRepository(context);

            context.Games.Add(new Game { Id = 1, Name = "Game 1", Genre = "Rpg", DeveloperName = "Some studio", ReleaseDate = new DateOnly(2020, 1, 1) });
            context.Games.Add(new Game { Id = 2, Name = "Game 2", Genre = "Rpg", DeveloperName = "Some studio", ReleaseDate = new DateOnly(2020, 1, 1) });

            context.Reviews.Add(new Review { Title = "Title", Content = "Content", GameId = 2, Rating = 8 });

            context.UserGames.Add(new UserGame { GameId = 1, AppUserId = "user-1" });
            context.UserGames.Add(new UserGame { GameId = 2, AppUserId = "user-1" });

            await context.SaveChangesAsync();

            var result = await repo.GetUserLibraryAsync("user-1", new LibraryQueryObject { MinAverageRating = 5 });

            Assert.Single(result);
            Assert.Equal(2, result[0].Id);
        }

        [Fact]
        public async Task GetUserLibraryAsync_ShouldFilterByMaxAverageRating()
        {
            var context = TestHelpers.CreateInMemoryContext();
            var repo = new LibraryRepository(context);

            context.Games.Add(new Game { Id = 1, Name = "Game 1", Genre = "Rpg", DeveloperName = "Some studio", ReleaseDate = new DateOnly(2020, 1, 1) });
            context.Games.Add(new Game { Id = 2, Name = "Game 2", Genre = "Rpg", DeveloperName = "Some studio", ReleaseDate = new DateOnly(2020, 1, 1) });

            context.Reviews.Add(new Review { Title = "Title", Content = "Content", GameId = 1, Rating = 4 });
            context.Reviews.Add(new Review { Title = "Title", Content = "Content", GameId = 2, Rating = 8 });

            context.UserGames.Add(new UserGame { GameId = 1, AppUserId = "user-1" });
            context.UserGames.Add(new UserGame { GameId = 2, AppUserId = "user-1" });

            await context.SaveChangesAsync();

            var result = await repo.GetUserLibraryAsync("user-1", new LibraryQueryObject { MaxAverageRating = 6 });

            Assert.Single(result);
            Assert.Equal(1, result[0].Id);
        }

        [Fact]
        public async Task GetUserLibraryAsync_ShouldFilterByGameReleasedAfterAndBefore()
        {
            var context = TestHelpers.CreateInMemoryContext();
            var repo = new LibraryRepository(context);

            context.Games.Add(new Game { Id = 1, Name = "Game 1", Genre = "Rpg", DeveloperName = "Some studio", ReleaseDate = new DateOnly(2020, 1, 1) });
            context.Games.Add(new Game { Id = 2, Name = "Game 2", Genre = "Rpg", DeveloperName = "Some studio", ReleaseDate = new DateOnly(2022, 1, 1) });
            context.Games.Add(new Game { Id = 3, Name = "Game 3", Genre = "Rpg", DeveloperName = "Some studio", ReleaseDate = new DateOnly(2025, 1, 1) });

            context.UserGames.Add(new UserGame { GameId = 1, AppUserId = "user-1" });
            context.UserGames.Add(new UserGame { GameId = 2, AppUserId = "user-1" });
            context.UserGames.Add(new UserGame { GameId = 3, AppUserId = "user-1" });

            await context.SaveChangesAsync();

            var result = await repo.GetUserLibraryAsync("user-1", new LibraryQueryObject {
                GameReleasedAfter = new DateOnly(2021, 1, 1), GameReleasedBefore = new DateOnly(2024, 1, 1) });

            Assert.Single(result);
            Assert.Equal(2, result[0].Id);
        }

        [Fact]
        public async Task GetUserLibraryAsync_ShouldFilterByAddedAfterAndBefore()
        {
            var context = TestHelpers.CreateInMemoryContext();
            var repo = new LibraryRepository(context);

            context.Games.Add(new Game { Id = 1, Name = "Game 1", Genre = "Rpg", DeveloperName = "Some studio", ReleaseDate = new DateOnly(2020, 1, 1) });
            context.Games.Add(new Game { Id = 2, Name = "Game 2", Genre = "Rpg", DeveloperName = "Some studio", ReleaseDate = new DateOnly(2020, 1, 1) });
            context.Games.Add(new Game { Id = 3, Name = "Game 3", Genre = "Rpg", DeveloperName = "Some studio", ReleaseDate = new DateOnly(2020, 1, 1) });

            context.UserGames.Add(new UserGame { GameId = 1, AppUserId = "user-1", AddedOn = new DateTime(2020, 1, 1) });
            context.UserGames.Add(new UserGame { GameId = 2, AppUserId = "user-1", AddedOn = new DateTime(2022, 1, 1) });
            context.UserGames.Add(new UserGame { GameId = 3, AppUserId = "user-1", AddedOn = new DateTime(2025, 1, 1) });

            await context.SaveChangesAsync();

            var result = await repo.GetUserLibraryAsync("user-1", new LibraryQueryObject {
                AddedAfter = new DateTime(2021, 1, 1), AddedBefore = new DateTime(2024, 1, 1) });

            Assert.Single(result);
            Assert.Equal(2, result[0].Id);
        }

        [Fact]
        public async Task GetUserLibraryAsync_ShouldSortByAdditionDate()
        {
            var context = TestHelpers.CreateInMemoryContext();
            var repo = new LibraryRepository(context);

            context.Games.Add(new Game { Id = 1, Name = "Game 1", Genre = "Rpg", DeveloperName = "Some studio", ReleaseDate = new DateOnly(2020, 1, 1) });
            context.Games.Add(new Game { Id = 2, Name = "Game 2", Genre = "Rpg", DeveloperName = "Some studio", ReleaseDate = new DateOnly(2020, 1, 1) });

            context.UserGames.Add(new UserGame { GameId = 1, AppUserId = "user-1", AddedOn = new DateTime(2022, 1, 1) });
            context.UserGames.Add(new UserGame { GameId = 2, AppUserId = "user-1", AddedOn = new DateTime(2020, 1, 1) });

            await context.SaveChangesAsync();

            var result = await repo.GetUserLibraryAsync("user-1", new LibraryQueryObject { SortBy = "AddedOn" });

            Assert.Equal(2, result[0].Id);
            Assert.Equal(1, result[1].Id);
        }

        [Fact]
        public async Task GetUserLibraryAsync_ShouldSortByAdditionDateDescending()
        {
            var context = TestHelpers.CreateInMemoryContext();
            var repo = new LibraryRepository(context);

            context.Games.Add(new Game { Id = 1, Name = "Game 1", Genre = "Rpg", DeveloperName = "Some studio", ReleaseDate = new DateOnly(2020, 1, 1) });
            context.Games.Add(new Game { Id = 2, Name = "Game 2", Genre = "Rpg", DeveloperName = "Some studio", ReleaseDate = new DateOnly(2020, 1, 1) });

            context.UserGames.Add(new UserGame { GameId = 1, AppUserId = "user-1", AddedOn = new DateTime(2020, 1, 1) });
            context.UserGames.Add(new UserGame { GameId = 2, AppUserId = "user-1", AddedOn = new DateTime(2022, 1, 1) });

            await context.SaveChangesAsync();

            var result = await repo.GetUserLibraryAsync("user-1", new LibraryQueryObject { SortBy = "AddedOn", IsDescending = true });

            Assert.Equal(2, result[0].Id);
            Assert.Equal(1, result[1].Id);
        }

        [Fact]
        public async Task GetUserLibraryAsync_ShouldSortByRating()
        {
            var context = TestHelpers.CreateInMemoryContext();
            var repo = new LibraryRepository(context);

            context.Games.Add(new Game { Id = 1, Name = "Game 1", Genre = "Rpg", DeveloperName = "Some studio", ReleaseDate = new DateOnly(2020, 1, 1) });
            context.Games.Add(new Game { Id = 2, Name = "Game 2", Genre = "Rpg", DeveloperName = "Some studio", ReleaseDate = new DateOnly(2020, 1, 1) });

            context.Reviews.Add(new Review { Title = "Title", Content = "Content", GameId = 1, Rating = 8 });
            context.Reviews.Add(new Review { Title = "Title", Content = "Content", GameId = 2, Rating = 4 });

            context.UserGames.Add(new UserGame { GameId = 1, AppUserId = "user-1", AddedOn = new DateTime(2020, 1, 1) });
            context.UserGames.Add(new UserGame { GameId = 2, AppUserId = "user-1", AddedOn = new DateTime(2020, 1, 1) });

            await context.SaveChangesAsync();

            var result = await repo.GetUserLibraryAsync("user-1", new LibraryQueryObject { SortBy = "Rating" });

            Assert.Equal(2, result[0].Id);
            Assert.Equal(1, result[1].Id);
        }

        [Fact]
        public async Task GetUserLibraryAsync_ShouldSortByRatingDescending()
        {
            var context = TestHelpers.CreateInMemoryContext();
            var repo = new LibraryRepository(context);

            context.Games.Add(new Game { Id = 1, Name = "Game 1", Genre = "Rpg", DeveloperName = "Some studio", ReleaseDate = new DateOnly(2020, 1, 1) });
            context.Games.Add(new Game { Id = 2, Name = "Game 2", Genre = "Rpg", DeveloperName = "Some studio", ReleaseDate = new DateOnly(2020, 1, 1) });

            context.Reviews.Add(new Review { Title = "Title", Content = "Content", GameId = 1, Rating = 4 });
            context.Reviews.Add(new Review { Title = "Title", Content = "Content", GameId = 2, Rating = 8 });

            context.UserGames.Add(new UserGame { GameId = 1, AppUserId = "user-1", AddedOn = new DateTime(2020, 1, 1) });
            context.UserGames.Add(new UserGame { GameId = 2, AppUserId = "user-1", AddedOn = new DateTime(2020, 1, 1) });

            await context.SaveChangesAsync();

            var result = await repo.GetUserLibraryAsync("user-1", new LibraryQueryObject { SortBy = "Rating", IsDescending = true });

            Assert.Equal(2, result[0].Id);
            Assert.Equal(1, result[1].Id);
        }

        [Fact]
        public async Task GetUserLibraryAsync_ShouldOnlyReturnGamesInThatUsersLibrary()
        {
            var context = TestHelpers.CreateInMemoryContext();
            var repo = new LibraryRepository(context);

            context.Games.Add(new Game { Id = 1, Name = "Game 1", Genre = "Rpg", DeveloperName = "Some studio", ReleaseDate = new DateOnly(2020, 1, 1) });
            context.Games.Add(new Game { Id = 2, Name = "Game 2", Genre = "Rpg", DeveloperName = "Some studio", ReleaseDate = new DateOnly(2020, 1, 1) });

            context.UserGames.Add(new UserGame { GameId = 1, AppUserId = "user-1", AddedOn = new DateTime(2020, 1, 1) });
            context.UserGames.Add(new UserGame { GameId = 2, AppUserId = "user-2", AddedOn = new DateTime(2020, 1, 1) });

            await context.SaveChangesAsync();

            var result = await repo.GetUserLibraryAsync("user-1", new LibraryQueryObject());

            Assert.Single(result);
            Assert.Equal(1, result[0].Id);
        }
    }
}