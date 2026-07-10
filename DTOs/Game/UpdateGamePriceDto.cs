using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GameLibraryAPI.DTOs.Game
{
    public class UpdateGamePriceDto
    {
        [Required]
        [Range(0.0, double.MaxValue, ErrorMessage = "Price must be greater than or equal to 0")]
        public double Price { get; set; }
    }
}