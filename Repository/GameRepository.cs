using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameLibraryAPI.Data;
using GameLibraryAPI.DTOs.Game;
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

        public async Task<List<Game>> GetAllAsync(string? genre = null)
        {
            var query = _context.Games.AsQueryable();

            if (!string.IsNullOrWhiteSpace(genre))
            {
                query = query.Where(g => g.Genre.ToLower() == genre.ToLower());
            }
            return await query.ToListAsync();
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
            game.ReleasedOn = updateDto.ReleasedOn;

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