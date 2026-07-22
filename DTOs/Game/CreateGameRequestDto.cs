using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GameLibraryAPI.DTOs.Game
{
    public class CreateGameRequestDto
    {
        [Required]
        [StringLength(150, MinimumLength = 1, ErrorMessage = "Game name must be between 1 and 150 characters.")]
        public string Name { get; set; } = "";
        [Required]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "Game genre must be between 1 and 50 characters.")]
        public string Genre { get; set; } = "";
        [Required]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "Game developer's name must be between 1 and 100 characters.")]
        public string DeveloperName { get; set; } = "";
        [Required(ErrorMessage = "Release date is required.")]
        public DateOnly ReleaseDate { get; set; }
    }
}