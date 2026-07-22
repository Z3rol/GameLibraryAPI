using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GameLibraryAPI.Helpers
{
    public class ReviewQueryObject : QueryObjectBase
    {
        [Range(0.1, 10.0)]
        public double? MinRating { get; set; }
        [Range(0.1, 10.0)]
        public double? MaxRating { get; set; }
        public DateTime? CreatedAfter { get; set; }
        public DateTime? CreatedBefore { get; set; }
        public string? SortBy { get; set; } = null;
        public bool IsDescending { get; set; } = false;
    }
}