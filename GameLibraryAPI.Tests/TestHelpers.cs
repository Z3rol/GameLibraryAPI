using Microsoft.AspNetCore.Identity;
using Moq;
using GameLibraryAPI.Models;

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
    }
}