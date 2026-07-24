using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameLibraryAPI.DTOs.Review;
using GameLibraryAPI.Helpers;
using GameLibraryAPI.Models;

namespace GameLibraryAPI.Interfaces
{
    public interface IReviewRepository
    {
        public Task<Review?> GetReviewByIdAsync(int reviewId);
        public Task<Review?> GetReviewByUserAndGameAsync(string userId, int gameId);
        public Task<List<ReviewDto>> GetReviewsByGameIdAsync(int gameId, ReviewQueryObject query);
        public Task<List<ReviewDto>> GetReviewsByUsernameAsync(string username, ReviewQueryObject query);
        public Task<Review> CreateReviewAsync(Review review);
        public Task<Review> UpdateReviewAsync(Review review, UpdateReviewRequestDto updateDto);
        public Task<Review?> DeleteReviewAsync(int reviewId);
        public Task<bool> UserHasReviewedGameAsync(string userId, int gameId);
    }
}