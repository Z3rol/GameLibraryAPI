using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameLibraryAPI.DTOs.Account;
using GameLibraryAPI.Models;
using GameLibraryAPI.Repository;
using GameLibraryAPI.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GameLibraryAPI.Controllers
{
    [Route("api/account")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ITokenService _tokenService;

        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, ITokenService tokenService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);

                var appUser = new AppUser
                {
                    UserName = registerDto.UserName,
                    Email = registerDto.Email
                };

                var createdUser = await _userManager .CreateAsync(appUser, registerDto.Password);

                if (createdUser.Succeeded)
                {
                    var roleResult = await _userManager.AddToRoleAsync(appUser, "User");

                    if (roleResult.Succeeded)
                    {
                        return Ok("User registered succsessfully");
                    }
                    else
                    {
                        return StatusCode(500, roleResult.Errors);
                    }
                }
                else
                {
                    return StatusCode(500, createdUser.Errors);
                }
            }
            catch(Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var user = await _userManager.FindByNameAsync(loginDto.UserNameOrEmail) ??
                await _userManager.FindByEmailAsync(loginDto.UserNameOrEmail);

            if (user == null)
            {
                return Unauthorized("Invalid username or password");
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);

            if (!result.Succeeded)
            {
                return Unauthorized("Invalid username or password");
            }

            var roles = await _userManager.GetRolesAsync(user);

            var token = _tokenService.CreateToken(user, roles);

            return Ok(new
            {
                UserName = user.UserName,
                Email = user.Email,
                Token = token
            });
        }
    }
}