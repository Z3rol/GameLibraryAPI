using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameLibraryAPI.Data;
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

        public async Task<List<Game>> GetUserLibraryAsync(string userId)
        {
            return await _context.UserGames
                .Where(ug => ug.AppUserId == userId)
                .Select(ug => ug.Game)
                .ToListAsync();
        }
    }
}