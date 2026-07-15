using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using GameLibraryAPI.Data;
using GameLibraryAPI.Extensions;
using GameLibraryAPI.Interfaces;
using GameLibraryAPI.Mappers;
using GameLibraryAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GameLibraryAPI.Controllers
{
    [Route("api/users/{username}/library")]
    [ApiController]
    [Authorize]
    public class LibraryController : ControllerBase
    {
        private readonly ILibraryRepository _libraryRepo;
        private readonly UserManager<AppUser> _userManager;
        public LibraryController(ILibraryRepository libraryRepo, UserManager<AppUser> userManager)
        {
            _libraryRepo = libraryRepo;
            _userManager = userManager;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetUserLibrary([FromRoute] string userName)
        {
            var user = await _userManager.FindByNameAsync(userName);
            if (user == null) return NotFound($"User '{userName}' does not exist.");

            var userGames = await _libraryRepo.GetUserLibraryAsync(user.Id);

            var userGamesDto = userGames.Select(ug => ug.ToGameDto());

            return Ok(userGamesDto);
        }

        [HttpPost("{gameId}")]
        public async Task<IActionResult> AddGameToLibrary([FromRoute] int gameId)
        {
            var userName = User.GetUserName();

            var user = await _userManager.FindByNameAsync(userName);
            if (user == null) return NotFound("User not found.");

            var userOwnsGame = await _libraryRepo.UserOwnsGameAsync(user.Id, gameId);
            if (userOwnsGame) return BadRequest("You already own this game.");

            var userGame = new UserGame
            {
                AppUserId = user.Id,
                GameId = gameId
            };
            
            await _libraryRepo.AddGameToLibraryAsync(userGame);

            return Ok("Game successfully added to your library.");
        }
    }
}