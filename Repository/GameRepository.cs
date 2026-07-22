using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

            // Filtering
            if (!string.IsNullOrWhiteSpace(query.Name))
            {
                games = games.Where(g => g.Name.ToLower().Contains(query.Name.ToLower()));
            }

            if (!string.IsNullOrWhiteSpace(query.Genre))
            {
                games = games.Where(g => g.Genre.ToLower() == query.Genre.ToLower());
            }

            if (!string.IsNullOrWhiteSpace(query.DeveloperName))
            {
                games = games.Where(g => g.DeveloperName.ToLower() == query.DeveloperName.ToLower());
            }

            // Sorting
            if (!string.IsNullOrWhiteSpace(query.SortBy))
            {
                if (query.SortBy.Equals("ReleaseDate", StringComparison.OrdinalIgnoreCase))
                {
                    games = query.IsDescending ? games.OrderByDescending(g => g.ReleaseDate) : games.OrderBy(g => g.ReleaseDate);
                }
            }

            var skipPages = (query.PageNumber - 1) * query.PageSize;

            return await games
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
            return await _context.Games.FindAsync(id);
        }

        public async Task<Game> CreateAsync(Game gameModel)
        {
            await _context.Games.AddAsync(gameModel);
            await _context.SaveChangesAsync();
            return gameModel;
        }

        public async Task<Game> UpdateDatailsAsync(Game game, UpdateGameDetailsDto updateDto)
        {
            game.Name = updateDto.Name;
            game.Genre = updateDto.Genre;
            game.DeveloperName = updateDto.DeveloperName;
            game.ReleaseDate = updateDto.ReleaseDate;

            await _context.SaveChangesAsync();
            return game;
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