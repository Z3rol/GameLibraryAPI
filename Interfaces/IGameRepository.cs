using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameLibraryAPI.DTOs.Game;
using GameLibraryAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace GameLibraryAPI.Interfaces
{
    public interface IGameRepository
    {
        public Task<List<Game>> GetAllAsync();
        public Task<Game?> GetByIdAsync(int id);
        public Task<Game> CreateAsync(Game gameModel);
        public Task<Game?> UpdateDatailsAsync(int id, UpdateGameDetailsDto updateDto);
        public Task<Game?> UpdatePriceAsync(int id, double newPrice);
        public Task<Game?> DeleteAsync(int id);
        public Task<bool> GameExistsAsync(string name);
        public Task<bool> GameExistsAsync(int id);
    }
}