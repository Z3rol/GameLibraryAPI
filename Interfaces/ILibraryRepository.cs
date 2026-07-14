using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameLibraryAPI.Models;

namespace GameLibraryAPI.Interfaces
{
    public interface ILibraryRepository
    {
        public Task<List<Game>> GetUserLibraryAsync(string userId);
    }
}