using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GameLibraryAPI.Models
{
    public class Review
    {
        public int Id { get; set; }
        public string Title { get; set; } = "";
        public string Content { get; set; } = "";
        public double Rating { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
        public int GameId { get; set; }
        public Game Game { get; set; } = null!;
        public string AppUserId { get; set; } = "";
        public AppUser AppUser { get; set; } = null!;
    }
}