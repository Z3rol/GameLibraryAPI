using System.Security.Claims;
using GameLibraryAPI.Controllers;
using GameLibraryAPI.DTOs.Account;
using GameLibraryAPI.Interfaces;
using GameLibraryAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace GameLibraryAPI.Tests
{
    public class AccountControllerTests
    {
        //                //
        // Helper methods //
        //                //
        
        private void SetupControllerUser(AccountController controller, string username)
        {
            var claims = new List<Claim> { new Claim(ClaimTypes.Name, username) };
            var identity = new ClaimsIdentity(claims, "TestAuth");

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(identity)
                }
            };
        }
        
        //                //
        // Register tests //
        //                //

        [Fact]
        public async Task Register_ShouldReturnOk_WhenSuccess()
        {
            var mockUserManager = TestHelpers.MockUserManager();
            var mockSignInManager = TestHelpers.MockSignInManager(mockUserManager.Object);
            var mockTokenService = new Mock<ITokenService>();
            var mockLogger = new Mock<ILogger<AccountController>>();

            mockUserManager.Setup(m => m.CreateAsync(It.IsAny<AppUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);
            mockUserManager.Setup(m => m.AddToRoleAsync(It.IsAny<AppUser>(), "User"))
                .ReturnsAsync(IdentityResult.Success);

            var controller = new AccountController(mockUserManager.Object, mockSignInManager.Object, mockTokenService.Object, mockLogger.Object);

            var registerDto = new RegisterDto { UserName = "TestUser", Email = "test@test.com", Password = "Password1!" };

            var result = await controller.Register(registerDto);

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task Register_ShouldReturn500_WhenCreateFails()
        {
            var mockUserManager = TestHelpers.MockUserManager();
            var mockSignInManager = TestHelpers.MockSignInManager(mockUserManager.Object);
            var mockTokenService = new Mock<ITokenService>();
            var mockLogger = new Mock<ILogger<AccountController>>();

            mockUserManager.Setup(m => m.CreateAsync(It.IsAny<AppUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Password is too weak" }));

            var controller = new AccountController(mockUserManager.Object, mockSignInManager.Object, mockTokenService.Object, mockLogger.Object);

            var registerDto = new RegisterDto { UserName = "TestUser", Email = "test@test.com", Password = "Password1!" };

            var result = await controller.Register(registerDto);

            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, objectResult.StatusCode);
        }

        //             //
        // Login tests //
        //             //

        [Fact]
        public async Task Login_ShouldReturnUnauthorized_WhenUserDoesNotExist()
        {
            var mockLogger = new Mock<ILogger<AccountController>>();
            var mockUserManager = TestHelpers.MockUserManager();

            mockUserManager.Setup(m => m.FindByNameAsync("TestUser")).ReturnsAsync((AppUser)null!);

            var controller = new AccountController(mockUserManager.Object, null!, null!, mockLogger.Object);

            var loginDto = new LoginDto { UserNameOrEmail = "TestUser" };

            var result = await controller.Login(loginDto);

            Assert.IsType<UnauthorizedObjectResult>(result);
        }

        [Fact]
        public async Task Login_ShouldReturnUnauthorized_WhenWrongPassword()
        {
            var mockUserManager = TestHelpers.MockUserManager();
            var mockSignInManager = TestHelpers.MockSignInManager(mockUserManager.Object);
            var mockLogger = new Mock<ILogger<AccountController>>();

            var fakeUser = new AppUser { Id = "user-1", UserName = "TestUser"};

            mockUserManager.Setup(m => m.FindByNameAsync("TestUser")).ReturnsAsync(fakeUser);
            mockSignInManager.Setup(s => s.CheckPasswordSignInAsync(fakeUser, "Password1!", false)).ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Failed);

            var controller = new AccountController(mockUserManager.Object, mockSignInManager.Object, null!, mockLogger.Object);

            var loginDto = new LoginDto { UserNameOrEmail = "TestUser", Password = "Password1!"};

            var result = await controller.Login(loginDto);

            Assert.IsType<UnauthorizedObjectResult>(result);
        }

        [Fact]
        public async Task Login_ShouldReturnOkWithToken_WhenSuccess()
        {
            var mockUserManager = TestHelpers.MockUserManager();
            var mockSignInManager = TestHelpers.MockSignInManager(mockUserManager.Object);
            var mockTokenService = new Mock<ITokenService>();
            var mockLogger = new Mock<ILogger<AccountController>>();

            var fakeUser = new AppUser { Id = "user-1", UserName = "TestUser" };

            mockUserManager.Setup(m => m.FindByNameAsync("TestUser")).ReturnsAsync(fakeUser);
            mockSignInManager.Setup(s => s.CheckPasswordSignInAsync(fakeUser, "Password1!", false))
                .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Success);
            mockUserManager.Setup(m => m.GetRolesAsync(fakeUser)).ReturnsAsync(new List<string> { "User" });
            mockTokenService.Setup(t => t.CreateToken(fakeUser, It.IsAny<IList<string>>())).Returns("fake-jwt-token");

            var controller = new AccountController(mockUserManager.Object, mockSignInManager.Object, mockTokenService.Object, mockLogger.Object);

            var loginDto = new LoginDto { UserNameOrEmail = "TestUser", Password = "Password1!" };

            var result = await controller.Login(loginDto);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var tokenValue = okResult.Value!.GetType().GetProperty("Token")!.GetValue(okResult.Value) as string;

            Assert.Equal("fake-jwt-token", tokenValue);
        }

        //                        //
        // Promote to Admin tests //
        //                        //
        
        [Fact]
        public async Task PromoteToAdmin_ShouldReturnNotFound_WhenUserDoesNotExist()
        {
            var mockUserManager = TestHelpers.MockUserManager();
            
            mockUserManager.Setup(m => m.FindByNameAsync("TestUser")).ReturnsAsync((AppUser)null!);

            var controller = new AccountController(mockUserManager.Object, null!, null!, null!);

            var result = await controller.PromoteToAdmin("TestUser");

            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task PromoteToAdmin_ShouldReturnBadRequest_WhenUserAlreadyAdmin()
        {
            var mockUserManager = TestHelpers.MockUserManager();

            var fakeUser = new AppUser { Id = "user-1", UserName = "TestUser" };
            
            mockUserManager.Setup(m => m.FindByNameAsync("TestUser")).ReturnsAsync(fakeUser);
            mockUserManager.Setup(m => m.IsInRoleAsync(fakeUser, "Admin")).ReturnsAsync(true);

            var controller = new AccountController(mockUserManager.Object, null!, null!, null!);

            var result = await controller.PromoteToAdmin("TestUser");

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task PromoteToAdmin_ShouldReturnBadRequestWithErrors_WhenAddToRoleFails()
        {
            var mockUserManager = TestHelpers.MockUserManager();

            var fakeUser = new AppUser { Id = "user-1", UserName = "TestUser" };
            var fakeError = new IdentityError 
            { 
                Code = "RoleError", 
                Description = "Failed to add role" 
            };
            
            mockUserManager.Setup(m => m.FindByNameAsync("TestUser")).ReturnsAsync(fakeUser);
            mockUserManager.Setup(m => m.IsInRoleAsync(fakeUser, "Admin")).ReturnsAsync(false);
            mockUserManager.Setup(m => m.AddToRoleAsync(fakeUser, "Admin")).ReturnsAsync(IdentityResult.Failed(fakeError));

            var controller = new AccountController(mockUserManager.Object, null!, null!, null!);

            var result = await controller.PromoteToAdmin("TestUser");

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);

            var returnedErrors = Assert.IsAssignableFrom<IEnumerable<IdentityError>>(badRequestResult.Value);

            Assert.Single(returnedErrors);
            Assert.Contains(returnedErrors, e => e.Description == "Failed to add role");
        }

        [Fact]
        public async Task PromoteToAdmin_ShouldReturnOk_WhenSuccess()
        {
            var mockUserManager = TestHelpers.MockUserManager();
            var mockLogger = new Mock<ILogger<AccountController>>();

            var fakeUser = new AppUser { Id = "user-1", UserName = "TestUser" };
            
            mockUserManager.Setup(m => m.FindByNameAsync("TestUser")).ReturnsAsync(fakeUser);
            mockUserManager.Setup(m => m.IsInRoleAsync(fakeUser, "Admin")).ReturnsAsync(false);
            mockUserManager.Setup(m => m.AddToRoleAsync(fakeUser, "Admin")).ReturnsAsync(IdentityResult.Success);

            var controller = new AccountController(mockUserManager.Object, null!, null!, mockLogger.Object);
            SetupControllerUser(controller, "TestUser");

            var result = await controller.PromoteToAdmin("TestUser");

            Assert.IsType<OkObjectResult>(result);
        }
    }
}