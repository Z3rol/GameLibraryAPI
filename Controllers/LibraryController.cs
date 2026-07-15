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
        private readonly IGameRepository _gameRepo;
        public LibraryController(ILibraryRepository libraryRepo, UserManager<AppUser> userManager, IGameRepository gameRepo)
        {
            _libraryRepo = libraryRepo;
            _userManager = userManager;
            _gameRepo = gameRepo;
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

            var gameExists = await _gameRepo.GameExistsAsync(gameId);
            if (!gameExists) return NotFound("Game does not exist");

            var userOwnsGame = await _libraryRepo.UserOwnsGameAsync(user.Id, gameId);
            if (userOwnsGame) return BadRequest("You already own this game.");
            
            await _libraryRepo.AddGameToLibraryAsync(user.Id, gameId);
            return Ok("Game successfully added to your library.");
        }

        [HttpDelete("{gameId}")]
        public async Task<IActionResult> RemoveGameFromLibrary([FromRoute] int gameId)
        {
            var username = User.GetUserName();

            var user = await _userManager.FindByNameAsync(username);
            if (user == null) return NotFound("User not found.");

            var userOwnsGame = await _libraryRepo.UserOwnsGameAsync(user.Id, gameId);
            if (!userOwnsGame) return BadRequest("This game is not in your library");

            await _libraryRepo.RemoveGameFromLibraryAsync(user.Id, gameId);
            return NoContent();
        }
    }
}