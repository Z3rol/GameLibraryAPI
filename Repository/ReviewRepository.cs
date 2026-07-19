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

        public async Task<Review?> GetReviewByIdAsync(int reviewId)
        {
            return await _context.Reviews.FindAsync(reviewId);
        }

        public async Task<List<Review>> GetReviewsByGameIdAsync(int gameId)
        {
            return await _context.Reviews
                .Where(r => r.GameId == gameId)
                .Include(r => r.AppUser)
                .ToListAsync();
        }

        public async Task<List<Review>> GetReviewsByUsernameAsync(string username)
        {
            return await _context.Reviews
                .Include(r => r.AppUser)
                .Include(r => r.Game)
                .Where(r => r.AppUser.UserName!.ToLower() == username.ToLower())
                .ToListAsync();
        }

        public async Task<Review> CreateReviewAsync(Review review)
        {
            await _context.Reviews.AddAsync(review);
            await _context.SaveChangesAsync();
            return review;
        }

        public async Task<Review?> DeleteReviewAsync(int reviewId)
        {
            var review = await _context.Reviews.FindAsync(reviewId);
            if (review == null) return null;

            _context.Remove(review);
            await _context.SaveChangesAsync();
            return review;
        }
    }
}