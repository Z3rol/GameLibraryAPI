namespace GameLibraryAPI.DTOs.Game
{
    public class GameDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Genre { get; set; } = "";
        public string DeveloperName { get; set; } = "";
        public DateOnly ReleaseDate { get; set; }
        public double AverageRating { get; set; }
    }
}