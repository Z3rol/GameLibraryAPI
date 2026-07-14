using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameLibraryAPI.Data;
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

            return Ok(userGames);
        }
    }
}