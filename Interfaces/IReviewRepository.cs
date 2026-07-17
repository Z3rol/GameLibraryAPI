using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameLibraryAPI.Models;

namespace GameLibraryAPI.Interfaces
{
    public interface IReviewRepository
    {
        public Task<List<Review>> GetReviewsByGameIdAsync(int gameId);
        public Task<List<Review>> GetReviewsByUsernameAsync(string username);
        public Task<Review> CreateReviewAsync(Review review);
    }
}