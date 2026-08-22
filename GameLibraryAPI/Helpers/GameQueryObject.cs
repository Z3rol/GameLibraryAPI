using System.ComponentModel.DataAnnotations;

namespace GameLibraryAPI.Helpers
{
    public class GameQueryObject : QueryObjectBase
    {
        public string? Name { get; set; }
        public string? Genre { get; set; }
        public string? DeveloperName { get; set; }
        public DateOnly? ReleasedAfter { get; set; }
        public DateOnly? ReleasedBefore { get; set; }
        [Range(0.1, 10.0)]
        public double? MinAverageRating { get; set; }
        [Range(0.1, 10.0)]
        public double? MaxAverageRating { get; set; }
        public string? SortBy { get; set; } = null;
        public bool IsDescending { get; set; } = false;
    }
}