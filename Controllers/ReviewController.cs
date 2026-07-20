using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameLibraryAPI.DTOs.Review;
using GameLibraryAPI.Extensions;
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
        public ReviewController(IReviewRepository reviewRepo, IGameRepository gameRepo, UserManager<AppUser> userManager)
        {
            _reviewRepo = reviewRepo;
            _gameRepo = gameRepo;
            _userManager = userManager;
        }

        [HttpGet("game/{gameId:int}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetReviewsByGameId([FromRoute] int gameId)
        {
            var gameExists = await _gameRepo.GameExistsAsync(gameId);
            if (!gameExists) return NotFound("Game does not exist");

            var reviews = await _reviewRepo.GetReviewsByGameIdAsync(gameId);
            var reviewsDto = reviews.Select(r => r.ToReviewDto());

            return Ok(reviewsDto);
        }

        [HttpGet("user/{username}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetReviewsByUsername([FromRoute] string username)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user == null) return NotFound($"User '{username}' does not exist");

            var reviews = await _reviewRepo.GetReviewsByUsernameAsync(username);
            var reviewsDto = reviews.Select(r => r.ToReviewDto());

            return Ok(reviewsDto);
        }

        [HttpPost]
        public async Task<IActionResult> CreateReview([FromBody] CreateReviewRequestDto createDto)
        {
            var username = User.GetUserName();
            if (string.IsNullOrWhiteSpace(username)) return Unauthorized("Could not extract username from token claims");

            var appUser = await _userManager.FindByNameAsync(username);
            if (appUser == null) return Unauthorized("User context not found");

            var gameExists = await _gameRepo.GameExistsAsync(createDto.GameId ?? 0);
            if (!gameExists) return NotFound("Game does not exist");

            var reviewModel = createDto.ToReviewFromCreate(appUser.Id);

            await _reviewRepo.CreateReviewAsync(reviewModel);
            
            return CreatedAtAction(nameof(GetReviewsByGameId), new { gameId = reviewModel.GameId }, reviewModel.ToReviewDto());
        }

        [HttpPut("{reviewId:int}")]
        public async Task<IActionResult> UpdateReview([FromRoute] int reviewId, [FromBody] UpdateReviewRequestDto updateDto)
        {
            var hasAnyValue = typeof(UpdateReviewRequestDto)
                .GetProperties()
                .Any(p => p.GetValue(updateDto) != null);
            if (!hasAnyValue) return BadRequest("Atleast one field must be provided");

            var username = User.GetUserName();
            if (string.IsNullOrWhiteSpace(username)) return Unauthorized("Could not extract username from token claims");

            var appUser = await _userManager.FindByNameAsync(username);
            if (appUser == null) return Unauthorized("User context not found");

            var review = await _reviewRepo.GetReviewByIdAsync(reviewId);
            if (review == null) return NotFound("Review does not exist");

            if (review.AppUserId != appUser.Id) return Forbid();

            var updatedReview = await _reviewRepo.UpdateReviewAsync(review, updateDto);
            return Ok(updatedReview.ToReviewDto());
        }

        [HttpDelete("{reviewId:int}")]
        public async Task<IActionResult> DeleteReview([FromRoute] int reviewId)
        {
            var username = User.GetUserName();
            if (string.IsNullOrWhiteSpace(username)) return Unauthorized("Could not extract username from token claims");

            var appUser = await _userManager.FindByNameAsync(username);
            if (appUser == null) return Unauthorized("User context not found");

            var review = await _reviewRepo.GetReviewByIdAsync(reviewId);
            if (review == null) return NotFound("Review does not exist");

            if (review.AppUserId != appUser.Id) return Forbid();

            await _reviewRepo.DeleteReviewAsync(reviewId);
            return NoContent();
        }
    }
}