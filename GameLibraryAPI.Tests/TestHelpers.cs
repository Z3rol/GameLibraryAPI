using Microsoft.AspNetCore.Identity;
using Moq;
using GameLibraryAPI.Models;
using Microsoft.AspNetCore.Http;
using GameLibraryAPI.Data;
using Microsoft.EntityFrameworkCore;

namespace GameLibraryAPI.Tests
{
    public static class TestHelpers
    {
        public static Mock<UserManager<AppUser>> MockUserManager()
        {
            var store = new Mock<IUserStore<AppUser>>();
            return new Mock<UserManager<AppUser>>(
                store.Object, null!, null!, null!, null!, null!, null!, null!, null!);
        }

        public static Mock<SignInManager<AppUser>> MockSignInManager(UserManager<AppUser> userManager)
        {
            var contextAccessor = new Mock<IHttpContextAccessor>();
            var claimsFactory = new Mock<IUserClaimsPrincipalFactory<AppUser>>();
            return new Mock<SignInManager<AppUser>>(userManager, contextAccessor.Object, claimsFactory.Object, null!, null!, null!, null!);
        }

        public static ApplicationDbContext CreateInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString()) // unique DB name per call
                .Options;

            return new ApplicationDbContext(options);
        }
    }
}