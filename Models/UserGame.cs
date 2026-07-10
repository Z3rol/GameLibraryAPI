using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GameLibraryAPI.Models
{
    public class UserGame
    {
        public string AppUserId { get; set; } = "";
        public AppUser AppUser { get; set; } = null!;
        public int GameId { get; set; }
        public Game Game { get; set; } = null!;
    }
}