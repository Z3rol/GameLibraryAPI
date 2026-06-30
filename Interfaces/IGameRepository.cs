using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameLibraryAPI.Models;

namespace GameLibraryAPI.Interfaces
{
    public interface IGameRepository
    {
        public Task<List<Game>> GetAllAsync();
    }
}