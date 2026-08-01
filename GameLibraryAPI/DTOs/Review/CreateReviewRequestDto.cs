using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GameLibraryAPI.DTOs.Review
{
    public class CreateReviewRequestDto
    {
        [Required]
        [Length(1, 50, ErrorMessage = "Title must be between 1 and 50 characters")]
        public string Title { get; set; } = "";
        [Required]
        [Length(1, 250, ErrorMessage = "Content must be between 1 and 50 characters")]
        public string Content { get; set; } = "";
        private double _rating;
        [Required]
        [Range(0.1, 10.0, ErrorMessage = "Rating should be in range from 0.1 to 10")]
        public double Rating
        {
            get => _rating;
            set => _rating = Math.Round(value, 1, MidpointRounding.AwayFromZero) ;
        }
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Please provide a valid gameId")]
        public int? GameId { get; set; }
    }
}