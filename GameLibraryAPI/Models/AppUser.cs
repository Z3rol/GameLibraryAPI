using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace GameLibraryAPI.Models
{
    public class AppUser : IdentityUser
    {
        public List<UserGame> UserGames { get; set; } = [];
        public List<Review> Reviews { get; set; } = [];
    }
}