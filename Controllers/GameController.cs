using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameLibraryAPI.Mappers;
using GameLibraryAPI.Interfaces;
using Microsoft.AspNetCore.Mvc;
using GameLibraryAPI.DTOs.Game;
using Microsoft.AspNetCore.Authorization;

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
            if (game == null) return NotFound("Game not found");

            return Ok(game.ToGameDto());
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] CreateGameRequestDto gameDto)
        {
            var nameIsTaken = await _gameRepo.GameExistsAsync(gameDto.Name);
            if (nameIsTaken) return BadRequest("Name is already taken;");

            var gameModel = gameDto.ToGameFromCreate();

            await _gameRepo.CreateAsync(gameModel);
            return CreatedAtAction(nameof (GetById), new {id = gameModel.Id}, gameModel.ToGameDto());
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateDetails([FromRoute] int id, [FromBody] UpdateGameDetailsDto updateDto)
        {
            var existingGame = await _gameRepo.GetByIdAsync(id);
            if (existingGame == null) return NotFound("Game not found.");
            
            if (updateDto.Name != existingGame.Name)
            {
                var nameIsTaken = await _gameRepo.GameExistsAsync(updateDto.Name);
                if (nameIsTaken)
                {
                    return BadRequest("A game with this name already exists.");
                }
            }

            var updatedGame = await _gameRepo.UpdateDatailsAsync(id, updateDto);
            return Ok(updatedGame);
        }

        [HttpPatch("{id}/price")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdatePrice([FromRoute] int id, [FromBody] UpdateGamePriceDto updateDto)
        {
            var game = await _gameRepo.UpdatePriceAsync(id, updateDto.Price);
            if (game == null) return NotFound("Game not found.");

            return Ok(game.ToGameDto());
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var game = await _gameRepo.DeleteAsync(id);
            if (game == null) return NotFound("Game not found.");

            return NoContent();
        }
    }
}