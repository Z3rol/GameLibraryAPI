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
        public Task<Game?> DeleteAsync(int id);
    }
}