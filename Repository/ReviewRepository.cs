using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameLibraryAPI.Data;
using GameLibraryAPI.DTOs.Review;
using GameLibraryAPI.Helpers;
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
            return await _context.Reviews.Include(r => r.AppUser).FirstOrDefaultAsync(r => r.Id == reviewId);
        }

        public async Task<Review?> GetReviewByUserAndGameAsync(string userId, int gameId)
        {
            return await _context.Reviews
                .FirstOrDefaultAsync(r => r.AppUserId == userId && r.GameId == gameId);
        }

        public async Task<List<ReviewDto>> GetReviewsByGameIdAsync(int gameId, ReviewQueryObject query)
        {
            var reviews = _context.Reviews.Where(r => r.GameId == gameId).AsQueryable();

            // Filtering
            if (query.MinRating != null)
            {
                reviews = reviews.Where(r => r.Rating >= query.MinRating);
            }

            if (query.MaxRating != null)
            {
                reviews = reviews.Where(r => r.Rating <= query.MaxRating);
            }

            if (query.CreatedAfter != null)
            {
                reviews = reviews.Where(r => r.CreatedOn >= query.CreatedAfter);
            }

            if (query.CreatedBefore != null)
            {
                reviews = reviews.Where(r => r.CreatedOn <= query.CreatedBefore);
            }

            // Sorting
            if (!string.IsNullOrWhiteSpace(query.SortBy))
            {
                if (query.SortBy.Equals("Rating", StringComparison.OrdinalIgnoreCase))
                {
                    reviews = query.IsDescending ? reviews.OrderByDescending(r => r.Rating) : reviews.OrderBy(r => r.Rating);
                }
                else if (query.SortBy.Equals("CreationDate", StringComparison.OrdinalIgnoreCase))
                {
                    reviews = query.IsDescending ? reviews.OrderByDescending(r => r.CreatedOn) : reviews.OrderBy(r => r.CreatedOn);
                }
            }

            var skipPages = (query.PageNumber - 1) * query.PageSize;

            return await reviews
                .Select(r => new ReviewDto
                {
                    Id = r.Id,
                    Title = r.Title,
                    Content = r.Content,
                    Rating = r.Rating,
                    CreatedOn = r.CreatedOn,
                    CreatedBy = r.AppUser.UserName ?? "",
                    GameId = r.GameId
                })
                .Skip(skipPages)
                .Take(query.PageSize)
                .ToListAsync();
        }

        public async Task<List<ReviewDto>> GetReviewsByUserIdAsync(string userId, ReviewQueryObject query)
        {
            var reviews = _context.Reviews.Where(r => r.AppUser.Id == userId);

            // Filtering
            if (query.GameName != null)
            {
                reviews = reviews.Where(r => r.Game.Name.ToLower().Contains(query.GameName.ToLower()));
            }
            
            if (query.MinRating != null)
            {
                reviews = reviews.Where(r => r.Rating >= query.MinRating);
            }

            if (query.MaxRating != null)
            {
                reviews = reviews.Where(r => r.Rating <= query.MaxRating);
            }

            if (query.CreatedAfter != null)
            {
                reviews = reviews.Where(r => r.CreatedOn >= query.CreatedAfter);
            }

            if (query.CreatedBefore != null)
            {
                reviews = reviews.Where(r => r.CreatedOn <= query.CreatedBefore);
            }

            // Sorting
            if (!string.IsNullOrWhiteSpace(query.SortBy))
            {
                if (query.SortBy.Equals("Rating", StringComparison.OrdinalIgnoreCase))
                {
                    reviews = query.IsDescending ? reviews.OrderByDescending(r => r.Rating) : reviews.OrderBy(r => r.Rating);
                }
                else if (query.SortBy.Equals("CreationDate", StringComparison.OrdinalIgnoreCase))
                {
                    reviews = query.IsDescending ? reviews.OrderByDescending(r => r.CreatedOn) : reviews.OrderBy(r => r.CreatedOn);
                }
            }

            var skipPages = (query.PageNumber - 1) * query.PageSize;

            return await reviews
                .Select(r => new ReviewDto
                {
                    Id = r.Id,
                    Title = r.Title,
                    Content = r.Content,
                    Rating = r.Rating,
                    CreatedOn = r.CreatedOn,
                    CreatedBy = r.AppUser.UserName ?? "",
                    GameId = r.GameId,
                    GameName = r.Game.Name
                })
                .Skip(skipPages)
                .Take(query.PageSize)
                .ToListAsync();
        }

        public async Task<Review> CreateReviewAsync(Review review)
        {
            await _context.Reviews.AddAsync(review);
            await _context.SaveChangesAsync();
            return review;
        }

        public async Task<Review> UpdateReviewAsync(Review review, UpdateReviewRequestDto updateDto)
        {
            review.Title = updateDto.Title ?? review.Title;
            review.Content = updateDto.Content ?? review.Content;
            review.Rating = updateDto.Rating ?? review.Rating;

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

        public async Task<bool> UserHasReviewedGameAsync(string userId, int gameId)
        {
            return await _context.Reviews.AnyAsync(r => r.AppUserId == userId && r.GameId == gameId);
        }
    }
}