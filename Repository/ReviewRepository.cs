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
    public class ReviewRepository : IReviewRepository
    {
        private readonly ApplicationDbContext _context;
        public ReviewRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Review>> GetReviewsByGameIdAsync(int gameId)
        {
            return await _context.Reviews
                .Where(r => r.GameId == gameId)
                .Include(r => r.AppUser)
                .ToListAsync();
        }

        public async Task<List<Review>> GetReviewsByUserIdAsync(string userId)
        {
            return await _context.Reviews
                .Where(r => r.AppUserId == userId)
                .Include(r => r.Game)
                .Include(r => r.AppUser)
                .ToListAsync();
        }
    }
}