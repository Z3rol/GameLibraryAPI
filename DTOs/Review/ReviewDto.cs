using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GameLibraryAPI.DTOs.Review
{
    public class ReviewDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = "";
        public string Content { get; set; } = "";
        public double Rating { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; } = "";
        public int GameId { get; set; }
    }
}