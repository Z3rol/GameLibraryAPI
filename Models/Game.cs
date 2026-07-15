using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GameLibraryAPI.Models
{
    public class Game
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Genre { get; set; } = "";
        public string DeveloperName { get; set; } = "";
        public double Price { get; set; }
        public DateOnly ReleaseDate { get; set; }
        public List<UserGame> UserGames { get; set; } = [];
    }
}