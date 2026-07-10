using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GameLibraryAPI.DTOs.Game
{
    public class GameDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Genre { get; set; } = "";
        public string DeveloperName { get; set; } = "";
        [Required]
        [Range(0.0, double.MaxValue, ErrorMessage = "Price must be greater than or equal to 0")]
        public double Price { get; set; }
        public DateOnly ReleasedOn { get; set; }
    }
}