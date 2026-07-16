using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameLibraryAPI.DTOs.Review;
using GameLibraryAPI.Models;

namespace GameLibraryAPI.Mappers
{
    public static class ReviewMappers
    {
        public static ReviewDto ToReviewDto(this Review review)
        {
            return new ReviewDto
            {
                Id = review.Id,
                Title = review.Title,
                Content = review.Content,
                Rating = review.Rating,
                CreatedOn = review.CreatedOn,
                GameId = review.GameId
            };
        }
    }
}