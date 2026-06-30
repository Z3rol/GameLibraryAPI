using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameLibraryAPI.Mappers;
using GameLibraryAPI.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GameLibraryAPI.Controllers
{
    [Route("api/game")]
    [ApiController]
    public class GameController : ControllerBase
    {
        private readonly IGameRepository _gameRepo;
        public GameController(IGameRepository gameRepo)
        {
            _gameRepo = gameRepo;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var games = await _gameRepo.GetAllAsync();
            var gamesDto = games.Select(g => g.ToGameDto());

            return Ok(gamesDto);
        }
    }
}