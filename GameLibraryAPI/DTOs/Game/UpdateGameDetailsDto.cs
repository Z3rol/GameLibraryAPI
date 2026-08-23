using System.ComponentModel.DataAnnotations;

namespace GameLibraryAPI.DTOs.Game
{
    public class UpdateGameDetailsDto
    {
        private string _name = "";
        [Required]
        [StringLength(150, MinimumLength = 1, ErrorMessage = "Game name must be between 1 and 150 characters")]
        public string Name
        {
            get => _name;
            set => _name = value?.Trim() ?? "";
        }
        private string _genre = "";
        [Required]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "Game genre must be between 1 and 50 characters")]
        public string Genre
        {
            get => _genre;
            set => _genre = value?.Trim() ?? "";
        }
        private string _developerName = "";
        [Required]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "Game developer name must be between 1 and 100 characters")]
        public string DeveloperName
        {
            get => _developerName;
            set => _developerName = value?.Trim() ?? "";
        }
        [Required]
        public DateOnly ReleaseDate { get; set; }
    }
}