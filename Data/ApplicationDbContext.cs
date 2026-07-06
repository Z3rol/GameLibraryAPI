using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameLibraryAPI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GameLibraryAPI.Data
{
    public class ApplicationDbContext : IdentityDbContext<AppUser>
    {
        public ApplicationDbContext(DbContextOptions dbContextOptions) : base(dbContextOptions)
        {
            
        }

        public DbSet<Game> Games { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            List<IdentityRole> roles = new List<IdentityRole>
            {
                new IdentityRole 
                { 
                    Name = "Admin",
                    NormalizedName = "ADMIN",
                    Id = "AdminRole_Id_1",
                    ConcurrencyStamp = "AdminRole_Stamp_1"
                },
                new IdentityRole
                {
                    Name = "User",
                    NormalizedName = "USER",
                    Id = "UserRole_Id_2",
                    ConcurrencyStamp = "UserRole_Stamp_2"
                }
            };

            builder.Entity<IdentityRole>().HasData(roles);
        }
    }
}