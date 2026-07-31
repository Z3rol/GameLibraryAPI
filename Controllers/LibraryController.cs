using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using GameLibraryAPI.Data;
using GameLibraryAPI.Extensions;
using GameLibraryAPI.Helpers;
using GameLibraryAPI.Interfaces;
using GameLibraryAPI.Mappers;
using GameLibraryAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace GameLibraryAPI.Controllers
{
    [Route("api/users/{username}/library")]
    [ApiController]
    [Authorize]
    public class LibraryController : ControllerBase
    {
        private readonly ILibraryRepository _libraryRepo;
        private readonly UserManager<AppUser> _userManager;
        private readonly IGameRepository _gameRepo;
        private readonly ILogger<LibraryController> _logger;
        public LibraryController(
            ILibraryRepository libraryRepo, UserManager<AppUser> userManager,
            IGameRepository gameRepo, ILogger<LibraryController> logger)
        {
            _libraryRepo = libraryRepo;
            _userManager = userManager;
            _gameRepo = gameRepo;
            _logger = logger;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetUserLibrary([FromRoute] string userName, [FromQuery] LibraryQueryObject query)
        {
            var user = await _userManager.FindByNameAsync(userName);
            if (user == null)
            {
                _logger.LogWarning("Get library failed: user '{Username}' not found", userName);
                return NotFound($"User '{userName}' does not exist.");
            }

            var userGamesDto = await _libraryRepo.GetUserLibraryAsync(user.Id, query);

            return Ok(userGamesDto);
        }

        [HttpPost("~/api/library/{gameId}")]
        public async Task<IActionResult> AddGameToLibrary([FromRoute] int gameId)
        {
            var username = User.GetUserName();
            if (string.IsNullOrWhiteSpace(username))
            {
                _logger.LogWarning("Could not extract username from token claims for request to {Path}", Request.Path);
                return Unauthorized("Could not extract username from token claims");
            }

            var appUser = await _userManager.FindByNameAsync(username);
            if (appUser == null)
            {
                _logger.LogWarning("Token valid but no matching user found for username '{Username}'", username);
                return Unauthorized("User context not found");
            }

            var gameExists = await _gameRepo.GameExistsAsync(gameId);
            if (!gameExists)
            {
                _logger.LogWarning("Adding game to library failed: game {GameId} not found", gameId);
                return NotFound("Game does not exist");
            }

            var userOwnsGame = await _libraryRepo.UserOwnsGameAsync(appUser.Id, gameId);
            if (userOwnsGame)
            {
                _logger.LogWarning("Adding game to library failed: game {GameId} already on user '{Username}' account",
                    gameId, username);
                return BadRequest("You already own this game.");
            }

            await _libraryRepo.AddGameToLibraryAsync(appUser.Id, gameId);
            _logger.LogInformation("Game {GameId} added to '{Username}' library",
                gameId, username);

            return Ok("Game successfully added to your library.");
        }

        [HttpDelete("~/api/library/{gameId}")]
        public async Task<IActionResult> RemoveGameFromLibrary([FromRoute] int gameId)
        {
            var username = User.GetUserName();
            if (string.IsNullOrWhiteSpace(username))
            {
                _logger.LogWarning("Could not extract username from token claims for request to {Path}", Request.Path);
                return Unauthorized("Could not extract username from token claims");
            }

            var appUser = await _userManager.FindByNameAsync(username);
            if (appUser == null)
            {
                _logger.LogWarning("Token valid but no matching user found for username '{Username}'", username);
                return Unauthorized("User context not found");
            }

            var userOwnsGame = await _libraryRepo.UserOwnsGameAsync(appUser.Id, gameId);
            if (!userOwnsGame)
            {
                _logger.LogWarning("Removing game from library failed: game {GameId} not found on '{Username} account'",
                    gameId, username);
                return BadRequest("This game is not in your library");
            }

            await _libraryRepo.RemoveGameFromLibraryAsync(appUser.Id, gameId);
            _logger.LogInformation("Game {GameId} removed from '{Username}' account",
                gameId, username);

            return NoContent();
        }
    }
}