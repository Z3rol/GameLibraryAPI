using GameLibraryAPI.Data;
using GameLibraryAPI.DTOs.Game;
using GameLibraryAPI.Helpers;
using GameLibraryAPI.Interfaces;
using GameLibraryAPI.Mappers;
using GameLibraryAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace GameLibraryAPI.Repository
{
    public class GameRepository : IGameRepository
    {
        private readonly ApplicationDbContext _context;
        public GameRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<GameDto>> GetAllAsync(GameQueryObject query)
        {
            var games = _context.Games.AsQueryable();

            var projected = games.Select(g => new
            {
                Game = g,
                AverageRating = g.Reviews.Any()
                    ? Math.Round(g.Reviews.Average(r => r.Rating), 1, MidpointRounding.AwayFromZero)
                    : 0
            });

            // Filtering
            if (!string.IsNullOrWhiteSpace(query.Name))
            {
                projected = projected.Where(x => x.Game.Name.ToLower().Contains(query.Name.ToLower()));
            }

            if (!string.IsNullOrWhiteSpace(query.Genre))
            {
                projected = projected.Where(x => x.Game.Genre.ToLower() == query.Genre.ToLower());
            }

            if (!string.IsNullOrWhiteSpace(query.DeveloperName))
            {
                projected = projected.Where(x => x.Game.DeveloperName.ToLower().Contains(query.DeveloperName.ToLower()));
            }

            if (query.ReleasedAfter != null)
            {
                projected = projected.Where(x => x.Game.ReleaseDate >= query.ReleasedAfter);
            }

            if (query.ReleasedBefore != null)
            {
                projected = projected.Where(x => x.Game.ReleaseDate <= query.ReleasedBefore);
            }

            if (query.MinAverageRating != null)
            {
                projected = projected.Where(x => x.AverageRating >= query.MinAverageRating);
            }

            if (query.MaxAverageRating != null)
            {
                projected = projected.Where(x => x.AverageRating <= query.MaxAverageRating);
            }

            // Sorting
            if (!string.IsNullOrWhiteSpace(query.SortBy))
            {
                if (query.SortBy.Equals("ReleaseDate", StringComparison.OrdinalIgnoreCase))
                {
                    projected = query.IsDescending ? projected.OrderByDescending(x => x.Game.ReleaseDate) : projected.OrderBy(x => x.Game.ReleaseDate);
                }
                else if (query.SortBy.Equals("Rating", StringComparison.OrdinalIgnoreCase))
                {
                    projected = query.IsDescending ? projected.OrderByDescending(x => x.AverageRating) : projected.OrderBy(x => x.AverageRating);
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

        public async Task<GameDto?> GetByIdAsync(int id)
        {
            return await _context.Games
                .Where(g => g.Id == id)
                .Select(g => new GameDto
                {
                    Id = g.Id,
                    Name = g.Name,
                    Genre = g.Genre,
                    DeveloperName = g.DeveloperName,
                    ReleaseDate = g.ReleaseDate,
                    AverageRating = g.Reviews.Any()
                        ? Math.Round(g.Reviews.Average(r => r.Rating), 1, MidpointRounding.AwayFromZero)
                        : 0
                })
                .FirstOrDefaultAsync();
        }

        public async Task<Game?> GetGameEntityByIdAsync(int id)
        {
            return await _context.Games
                .Include(g => g.Reviews)
                .FirstOrDefaultAsync(g => g.Id == id);
        }

        public async Task<Game> CreateAsync(Game gameModel)
        {
            await _context.Games.AddAsync(gameModel);
            await _context.SaveChangesAsync();
            return gameModel;
        }

        public async Task<GameDto> UpdateDatailsAsync(Game game, UpdateGameDetailsDto updateDto)
        {
            game.Name = updateDto.Name;
            game.Genre = updateDto.Genre;
            game.DeveloperName = updateDto.DeveloperName;
            game.ReleaseDate = updateDto.ReleaseDate;

            await _context.SaveChangesAsync();

            var averageRating = game.Reviews.Any()
                ? Math.Round(game.Reviews.Average(r => r.Rating), 1, MidpointRounding.AwayFromZero)
                : 0;

            var gameDto = game.ToGameDto();
            gameDto.AverageRating = averageRating;

            return gameDto;
        }

        public async Task<Game?> DeleteAsync(int id)
        {
            var game = await _context.Games.FindAsync(id);
            if (game == null) return null;

            _context.Remove(game);
            await _context.SaveChangesAsync();
            return game;
        }

        public async Task<bool> GameExistsAsync(string name)
        {
            return await _context.Games.AnyAsync(g => g.Name.ToLower() == name.ToLower());
        }

        public async Task<bool> GameExistsAsync(int id)
        {
            return await _context.Games.AnyAsync(g => g.Id == id);
        }
    }
}