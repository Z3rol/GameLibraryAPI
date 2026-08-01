using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GameLibraryAPI.Helpers
{
    public class LibraryQueryObject : QueryObjectBase
    {
        public string? GameName { get; set; } = null;
        public string? Genre { get; set; } = null;
        public double? MinAverageRating { get; set; }
        public double? MaxAverageRating { get; set; }
        public DateOnly? GameReleasedAfter { get; set; }
        public DateOnly? GameReleasedBefore { get; set; }
        public DateTime? AddedAfter { get; set; }
        public DateTime? AddedBefore { get; set; }
        public string? SortBy { get; set; } = null;
        public bool IsDescending { get; set; } = false;
    }
}