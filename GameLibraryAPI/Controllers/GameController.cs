using GameLibraryAPI.Mappers;
using GameLibraryAPI.Interfaces;
using Microsoft.AspNetCore.Mvc;
using GameLibraryAPI.DTOs.Game;
using Microsoft.AspNetCore.Authorization;
using GameLibraryAPI.Helpers;
using GameLibraryAPI.Extensions;

namespace GameLibraryAPI.Controllers
{
    [Route("api/game")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class GameController : ControllerBase
    {
        private readonly IGameRepository _gameRepo;
        private readonly ILogger<GameController> _logger;
        public GameController(IGameRepository gameRepo, ILogger<GameController> logger)
        {
            _gameRepo = gameRepo;
            _logger = logger;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll([FromQuery] GameQueryObject query)
        {
            var gamesDto = await _gameRepo.GetAllAsync(query);
            
            return Ok(gamesDto);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var gameDto = await _gameRepo.GetByIdAsync(id);
            if (gameDto == null) return NotFound("Game not found");

            return Ok(gameDto);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateGameRequestDto gameDto)
        {
            var nameIsTaken = await _gameRepo.GameExistsAsync(gameDto.Name);
            if (nameIsTaken)
            {
                _logger.LogWarning("Game creation failed: name '{GameName}' already taken", gameDto.Name);
                return BadRequest("Name is already taken;");
            }

            var gameModel = gameDto.ToGameFromCreate();

            await _gameRepo.CreateAsync(gameModel);
            _logger.LogInformation("Game '{GameName}' (Id: {GameId}) created by {Username}",
                gameModel.Name, gameModel.Id, User.GetUserName() ?? "unknown");

            return CreatedAtAction(nameof (GetById), new {id = gameModel.Id}, gameModel.ToGameDto());
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDetails([FromRoute] int id, [FromBody] UpdateGameDetailsDto updateDto)
        {
            var existingGame = await _gameRepo.GetGameEntityByIdAsync(id);
            if (existingGame == null)
            {
                _logger.LogWarning("Update failed: game {GameId} not found", id);
                return NotFound("Game not found.");
            }
            
            if (updateDto.Name != existingGame.Name)
            {
                var nameIsTaken = await _gameRepo.GameExistsAsync(updateDto.Name);
                if (nameIsTaken)
                {
                    _logger.LogWarning("Game update failed: name '{GameName}' already taken", existingGame.Name);
                    return BadRequest("A game with this name already exists.");
                }
            }

            var updatedGameDto = await _gameRepo.UpdateDatailsAsync(existingGame, updateDto);
            _logger.LogInformation("Game {GameId} updated by {Username}",
                existingGame.Id, User.GetUserName() ?? "unknown");

            return Ok(updatedGameDto);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var game = await _gameRepo.DeleteAsync(id);
            if (game == null)
            {
                _logger.LogWarning("Delete failed: game {GameId} not found", id);
                return NotFound("Game not found.");
            }

            _logger.LogInformation("Game {GameId} deleted by {Username}",
                game.Id, User.GetUserName() ?? "unknown");

            return NoContent();
        }
    }
}