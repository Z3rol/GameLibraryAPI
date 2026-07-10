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

            if (game == null)
            {
                return NotFound("Game not found");
            }

            var gameDto = game.ToGameDto();

            return Ok(gameDto);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] CreateGameRequestDto gameDto)
        {
            // Prevent duplicates
            var nameIsTaken = await _gameRepo.GameExistsAsync(gameDto.Name);
            if (nameIsTaken)
            {
                return BadRequest("Name is already taken");
            }

            var gameModel = gameDto.ToGameFromCreate();
            await _gameRepo.CreateAsync(gameModel);
            return CreatedAtAction(nameof (GetById), new {id = gameModel.Id}, gameModel.ToGameDto());
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateDetails([FromRoute] int id, [FromBody] UpdateGameDetailsDto updateDto)
        {
            try
            {    
                var game = await _gameRepo.UpdateDatailsAsync(id, updateDto);
                if (game == null)
                    return NotFound("Game not found");

                return Ok(game.ToGameDto());
            }
            catch (InvalidOperationException ex) when (ex.Message == "NameIsTaken")
            {
                return BadRequest("Name is already taken");
            }
        }

        [HttpPatch("{id}/price")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdatePrice([FromRoute] int id, [FromBody] UpdateGamePriceDto updateDto)
        {
            var game = await _gameRepo.UpdatePriceAsync(id, updateDto.Price);

            if (game == null)
            {
                return NotFound("Game not found");
            }

            return Ok(game.ToGameDto());
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
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