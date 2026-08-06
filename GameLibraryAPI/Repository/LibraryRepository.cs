using GameLibraryAPI.Data;
using GameLibraryAPI.DTOs.Game;
using GameLibraryAPI.Helpers;
using GameLibraryAPI.Interfaces;
using GameLibraryAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace GameLibraryAPI.Repository
{
    public class LibraryRepository : ILibraryRepository
    {
        private readonly ApplicationDbContext _context;
        public LibraryRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<GameDto>> GetUserLibraryAsync(string userId, LibraryQueryObject query)
        {
            var userGames = _context.UserGames.Where(ug => ug.AppUserId == userId).AsQueryable();

            var projected = userGames.Select(ug => new
            {
                ug.Game,
                ug.AddedOn,
                AverageRating = ug.Game.Reviews.Any()
                    ? Math.Round(ug.Game.Reviews.Average(r => r.Rating), 1, MidpointRounding.AwayFromZero)
                    : 0
            });

            // Filtering
            if (query.GameName != null)
            {
                projected = projected.Where(x => x.Game.Name.ToLower().Contains(query.GameName.ToLower()));
            }

            if (query.Genre != null)
            {
                projected = projected.Where(x => x.Game.Genre.ToLower().Contains(query.Genre.ToLower()));
            }

            if (query.MinAverageRating != null)
            {
                projected = projected.Where(x => x.AverageRating >= query.MinAverageRating);
            }

            if (query.MaxAverageRating != null)
            {
                projected = projected.Where(x => x.AverageRating <= query.MaxAverageRating);
            }

            if (query.GameReleasedAfter != null)
            {
                projected = projected.Where(x => x.Game.ReleaseDate >= query.GameReleasedAfter);
            }

            if (query.GameReleasedBefore != null)
            {
                projected = projected.Where(x => x.Game.ReleaseDate <= query.GameReleasedBefore);
            }

            if (query.AddedAfter != null)
            {
                projected = projected.Where(x => x.AddedOn >= query.AddedAfter);
            }

            if (query.AddedBefore != null)
            {
                projected = projected.Where(x => x.AddedOn <= query.AddedBefore);
            }

            // Sorting
            if (!string.IsNullOrWhiteSpace(query.SortBy))
            {
                if (query.SortBy.Equals("GameReleaseDate", StringComparison.OrdinalIgnoreCase))
                {
                    projected = query.IsDescending ? projected.OrderByDescending(x => x.Game.ReleaseDate) : projected.OrderBy(x => x.Game.ReleaseDate);
                }
                else if (query.SortBy.Equals("AddedOn", StringComparison.OrdinalIgnoreCase))
                {
                    projected = query.IsDescending ? projected.OrderByDescending(x => x.AddedOn) : projected.OrderBy(x => x.AddedOn);
                }
                else if (query.SortBy.Equals("Rating", StringComparison.OrdinalIgnoreCase))
                {
                    projected = query.IsDescending
                        ? projected.OrderByDescending(x => x.AverageRating)
                        : projected.OrderBy(x => x.AverageRating);
                }
            }

            var skipPages = (query.PageNumber - 1) * query.PageSize;

            return await projected
                .Select(x => new GameDto
                {
                    Id = x.Game.Id,
                    Name = x.Game.Name,
                    Genre = x.Game.Genre,
                    DeveloperName = x.Game.DeveloperName,
                    ReleaseDate = x.Game.ReleaseDate,
                    AverageRating = x.AverageRating
                })
                .Skip(skipPages)
                .Take(query.PageSize)
                .ToListAsync();
        }

        public async Task<bool> UserOwnsGameAsync(string userId, int gameId)
        {
            return await _context.UserGames
                .AnyAsync(ug => ug.AppUserId == userId && ug.GameId == gameId);
        }

        public async Task<UserGame> AddGameToLibraryAsync(string userId, int gameId)
        {
            var userGame = new UserGame
            {
                AppUserId = userId,
                GameId = gameId
            };

            await _context.UserGames.AddAsync(userGame);
            await _context.SaveChangesAsync();
            return userGame;
        }

        public async Task<UserGame?> RemoveGameFromLibraryAsync(string userId, int gameId)
        {
            var userGame = await _context.UserGames.
                FirstOrDefaultAsync(ug => ug.GameId == gameId && ug.AppUserId == userId);

            if (userGame == null) return null;

            _context.UserGames.Remove(userGame);
            await _context.SaveChangesAsync();
            return userGame;
        }
    }
}