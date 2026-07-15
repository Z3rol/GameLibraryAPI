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

        public async Task<List<Game>> GetAllAsync(GameQueryObject query)
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
                if (query.SortBy.Equals("Price", StringComparison.OrdinalIgnoreCase))
                {
                    games = query.IsDescending ? games.OrderByDescending(g => g.Price) : games.OrderBy(g => g.Price);
                }
                else if (query.SortBy.Equals("ReleasedOn", StringComparison.OrdinalIgnoreCase))
                {
                    games = query.IsDescending ? games.OrderByDescending(g => g.ReleaseDate) : games.OrderBy(g => g.ReleaseDate);
                }
            }

            return await games.ToListAsync();
        }

        public async Task<Game?> GetByIdAsync(int id)
        {
            return await _context.Games.FindAsync(id);
        }

        public async Task<Game> CreateAsync(Game gameModel)
        {
            await _context.Games.AddAsync(gameModel);
            await _context.SaveChangesAsync();
            return gameModel;
        }

        public async Task<Game?> UpdateDatailsAsync(int id, UpdateGameDetailsDto updateDto)
        {
            var game = await _context.Games.FindAsync(id);
            if (game == null) return null;

            game.Name = updateDto.Name;
            game.Genre = updateDto.Genre;
            game.DeveloperName = updateDto.DeveloperName;
            game.ReleaseDate = updateDto.ReleaseDate;

            await _context.SaveChangesAsync();
            return game;
        }

        public async Task<Game?> UpdatePriceAsync(int id, double newPrice)
        {
            var game = await _context.Games.FindAsync(id);
            if (game == null) return null;

            game.Price = newPrice;
            
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