using System.ComponentModel.DataAnnotations;

namespace GameLibraryAPI.Helpers
{
    public class QueryObjectBase
    {
        [Range(1, int.MaxValue)]
        public int PageNumber { get; set; } = 1;
        [Range(1, 50)]
        public int PageSize { get; set; } = 20;
    }
}