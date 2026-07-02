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
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public double Price { get; set; }
    }
}