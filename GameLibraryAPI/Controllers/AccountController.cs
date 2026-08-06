using GameLibraryAPI.DTOs.Account;
using GameLibraryAPI.Models;
using GameLibraryAPI.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.RateLimiting;

namespace GameLibraryAPI.Controllers
{
    [Route("api/account")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ITokenService _tokenService;
        private readonly ILogger<AccountController> _logger;

        public AccountController(
            UserManager<AppUser> userManager, SignInManager<AppUser> signInManager,
            ITokenService tokenService, ILogger<AccountController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
            _logger = logger;
        }

        [HttpPost("register")]
        [EnableRateLimiting("AuthPolicy")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            var appUser = new AppUser
            {
                UserName = registerDto.UserName,
                Email = registerDto.Email
            };

            var createdUser = await _userManager .CreateAsync(appUser, registerDto.Password);

            if (!createdUser.Succeeded)
            {
                _logger.LogWarning("Registration failed for {UserName}: {Errors}",
                    registerDto.UserName, string.Join(", ", createdUser.Errors.Select(e => e.Description)));
                return StatusCode(500, createdUser.Errors);
            }

            var roleResult = await _userManager.AddToRoleAsync(appUser, "User");

            if (!roleResult.Succeeded)
            {
                _logger.LogWarning("Role assignment failed for {UserName}: {Errors}",
                    registerDto.UserName, string.Join(", ", roleResult.Errors.Select(e => e.Description)));
                return StatusCode(500, roleResult.Errors);
            }

            _logger.LogInformation("User {UserName} registered successfully", registerDto.UserName);
            return Ok("User registered successfully");
        }

        [HttpPost("login")]
        [EnableRateLimiting("AuthPolicy")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var user = await _userManager.FindByNameAsync(loginDto.UserNameOrEmail)
                ?? await _userManager.FindByEmailAsync(loginDto.UserNameOrEmail);

            if (user == null)
            {
                _logger.LogWarning("Login failed: no user found for {UserNameOrEmail}", loginDto.UserNameOrEmail);
                return Unauthorized("Invalid username or password");
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);

            if (!result.Succeeded)
            {
                _logger.LogWarning("Login failed: incorrect password for {UserNameOrEmail}", loginDto.UserNameOrEmail);
                return Unauthorized("Invalid username or password");
            }

            var roles = await _userManager.GetRolesAsync(user);
            var token = _tokenService.CreateToken(user, roles);

            _logger.LogInformation("User {UserName} logged in successfully", user.UserName);

            return Ok(new
            {
                UserName = user.UserName,
                Email = user.Email,
                Token = token
            });
        }

        [HttpPost("promote/{username}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PromoteToAdmin([FromRoute] string username)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user == null) return NotFound("User does not exist");

            var alreadyAdmin = await _userManager.IsInRoleAsync(user, "Admin");
            if (alreadyAdmin) return BadRequest("User is already an admin");

            var result = await _userManager.AddToRoleAsync(user, "Admin");
            if (!result.Succeeded) return BadRequest(result.Errors);

            _logger.LogInformation("User {UserName} promoted to admin by {PromotedBy}",
                user.UserName, User.Identity?.Name ?? "unknown");

            return Ok($"{username} is now an admin");
        }
    }
}