using GameLibraryAPI.DTOs.Review;
using GameLibraryAPI.Extensions;
using GameLibraryAPI.Helpers;
using GameLibraryAPI.Interfaces;
using GameLibraryAPI.Mappers;
using GameLibraryAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GameLibraryAPI.Controllers
{
    [Route("api/review")]
    [ApiController]
    [Authorize]
    public class ReviewController : ControllerBase
    {
        private readonly IReviewRepository _reviewRepo;
        private readonly IGameRepository _gameRepo;
        private readonly UserManager<AppUser> _userManager;
        private readonly ILogger<ReviewController> _logger;
        public ReviewController(
            IReviewRepository reviewRepo, IGameRepository gameRepo,
            UserManager<AppUser> userManager, ILogger<ReviewController> logger)
        {
            _reviewRepo = reviewRepo;
            _gameRepo = gameRepo;
            _userManager = userManager;
            _logger = logger;
        }

        [HttpGet("game/{gameId:int}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetReviewsByGameId([FromRoute] int gameId, [FromQuery] ReviewQueryObject query)
        {
            var gameExists = await _gameRepo.GameExistsAsync(gameId);
            if (!gameExists)
            {
                _logger.LogWarning("Get reviews failed: game {GameId} not found", gameId);
                return NotFound("Game does not exist");
            }

            var reviewsDto = await _reviewRepo.GetReviewsByGameIdAsync(gameId, query);

            return Ok(reviewsDto);
        }

        [HttpGet("user/{username}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetReviewsByUsername([FromRoute] string username, [FromQuery] ReviewQueryObject query)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
            {
                _logger.LogWarning("Get reviews failed: user {Username} not found", username);
                return NotFound($"User '{username}' does not exist");
            }

            var reviewsDto = await _reviewRepo.GetReviewsByUserIdAsync(user.Id, query);

            return Ok(reviewsDto);
        }

        [HttpPost]
        public async Task<IActionResult> CreateReview([FromBody] CreateReviewRequestDto createDto)
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

            var gameExists = await _gameRepo.GameExistsAsync(createDto.GameId ?? 0);
            if (!gameExists)
            {
                _logger.LogWarning("Review creation failed: game {GameId} not found", createDto.GameId);
                return NotFound("Game does not exist");
            }

            var reviewExists = await _reviewRepo.UserHasReviewedGameAsync(appUser.Id, createDto.GameId ?? 0);
            if (reviewExists)
            {
                _logger.LogWarning("Review creation failed: user '{Username}' already has a review for game {GameId}",
                    username, createDto.GameId);
                return BadRequest("User already has a review of this game");
            }

            var reviewModel = createDto.ToReviewFromCreate(appUser.Id);

            await _reviewRepo.CreateReviewAsync(reviewModel);
            _logger.LogInformation("Review created for game {GameId} by '{Username}'",
                createDto.GameId, username);

            return CreatedAtAction(nameof(GetReviewsByGameId), new { gameId = reviewModel.GameId }, reviewModel.ToReviewDto());
        }

        [HttpPut("{gameId:int}")]
        public async Task<IActionResult> UpdateReview([FromRoute] int gameId, [FromBody] UpdateReviewRequestDto updateDto)
        {
            var hasAnyValue = typeof(UpdateReviewRequestDto)
                .GetProperties()
                .Any(p => p.GetValue(updateDto) != null);
            if (!hasAnyValue) return BadRequest("Atleast one field must be provided");

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

            var review = await _reviewRepo.GetReviewByUserAndGameAsync(appUser.Id, gameId);
            if (review == null)
            {
                _logger.LogWarning("Review update failed: user '{Username}' doesnt have a review for game {GameId}",
                    username, gameId);
                return NotFound("You have not reviewed this game");
            }

            var updatedReview = await _reviewRepo.UpdateReviewAsync(review, updateDto);
            _logger.LogInformation("Review {ReviewId} updated by '{Username}' for game {GameId}",
                review.Id, username, gameId);

            return Ok(updatedReview.ToReviewDto());
        }

        [HttpDelete("{gameId:int}")]
        public async Task<IActionResult> DeleteReview([FromRoute] int gameId)
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

            var review = await _reviewRepo.GetReviewByUserAndGameAsync(appUser.Id, gameId);
            if (review == null)
            {
                _logger.LogWarning("Review deletion failed: game {GameId} not found", gameId);
                return NotFound("Review does not exist");
            }

            await _reviewRepo.DeleteReviewAsync(review.Id);
            _logger.LogInformation("Review {ReviewId} deleted by '{Username}'",
                review.Id, username);

            return NoContent();
        }
    }
}