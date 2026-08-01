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
                CreatedBy = review.AppUser?.UserName ?? "",
                GameId = review.GameId
            };
        }

        public static Review ToReviewFromCreate(this CreateReviewRequestDto createDto, string userId)
        {
            return new Review
            {
                Title = createDto.Title,
                Content = createDto.Content,
                Rating = createDto.Rating,
                GameId = createDto.GameId ?? 0,
                AppUserId = userId,
                CreatedOn = DateTime.UtcNow
            };
        }
    }
}