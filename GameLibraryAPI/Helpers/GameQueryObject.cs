using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GameLibraryAPI.Helpers
{
    public class GameQueryObject : QueryObjectBase
    {
        public string? Name { get; set; }
        public string? Genre { get; set; }
        public string? DeveloperName { get; set; }
        public string? SortBy { get; set; } = null;
        public bool IsDescending { get; set; } = false;
    }
}