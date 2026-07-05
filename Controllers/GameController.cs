using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameLibraryAPI.Mappers;
using GameLibraryAPI.Interfaces;
using Microsoft.AspNetCore.Mvc;
using GameLibraryAPI.DTOs.Game;

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

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var game = await _gameRepo.GetByIdAsync(id);

            if (game == null)
            {
                return NotFound("Game not found");
            }

            var gameDto = game.ToGameDto();

            return Ok(gameDto);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateGameRequestDto gameDto)
        {
            var gameModel = gameDto.ToGameFromCreate();
            await _gameRepo.CreateAsync(gameModel);
            return CreatedAtAction(nameof (GetById), new {id = gameModel.Id}, gameModel.ToGameDto());
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDetails([FromRoute] int id, [FromBody] UpdateGameDetailsDto newDatails)
        {
            var game = await _gameRepo.UpdateDatailsAsync(id, newDatails);

            if (game == null)
            {
                return NotFound("Game not found");
            }

            return Ok(game.ToGameDto());
        }

        [HttpPatch("{id}/price")]
        public async Task<IActionResult> UpdatePrice([FromRoute] int id, [FromBody] UpdateGamePriceDto newPrice)
        {
            var game = await _gameRepo.UpdatePriceAsync(id, newPrice.Price);

            if (game == null)
            {
                return NotFound("Game not found");
            }

            return Ok(game.ToGameDto());
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var game = await _gameRepo.DeleteAsync(id);

            if (game == null)
            {
                return NotFound("Game not found");
            }

            return NoContent();
        }
    }
}