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
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> dbContextOptions)
            : base(dbContextOptions) { }

        public DbSet<Game> Games { get; set; }
        public DbSet<UserGame> UserGames { get; set; }
        public DbSet<Review> Reviews { get; set; }

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


            builder.Entity<UserGame>()
                .HasKey(ug => new { ug.AppUserId, ug.GameId });

            builder.Entity<UserGame>()
                .HasOne(ug => ug.AppUser)
                .WithMany(ug => ug.UserGames)
                .HasForeignKey(ug => ug.AppUserId);

            builder.Entity<UserGame>()
                .HasOne(ug => ug.Game)
                .WithMany(ug => ug.UserGames)
                .HasForeignKey(ug => ug.GameId);
        }
    }
}