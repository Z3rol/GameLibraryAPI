using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        public ReviewController(IReviewRepository reviewRepo, IGameRepository gameRepo, UserManager<AppUser> userManager)
        {
            _reviewRepo = reviewRepo;
            _gameRepo = gameRepo;
            _userManager = userManager;
        }

        [HttpGet("game/{gameId:int}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetReviewsByGameId([FromRoute] int gameId, [FromQuery] ReviewQueryObject query)
        {
            var gameExists = await _gameRepo.GameExistsAsync(gameId);
            if (!gameExists) return NotFound("Game does not exist");

            var reviewsDto = await _reviewRepo.GetReviewsByGameIdAsync(gameId, query);;

            return Ok(reviewsDto);
        }

        [HttpGet("user/{username}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetReviewsByUsername([FromRoute] string username, [FromQuery] ReviewQueryObject query)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user == null) return NotFound($"User '{username}' does not exist");

            var reviewsDto = await _reviewRepo.GetReviewsByUsernameAsync(username, query);

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

            var reviewExists = await _reviewRepo.UserHasReviewedGameAsync(appUser.Id, createDto.GameId ?? 0);
            if (reviewExists) return BadRequest("User already has a review of this game");

            var reviewModel = createDto.ToReviewFromCreate(appUser.Id);

            await _reviewRepo.CreateReviewAsync(reviewModel);
            
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
            if (string.IsNullOrWhiteSpace(username)) return Unauthorized("Could not extract username from token claims");

            var appUser = await _userManager.FindByNameAsync(username);
            if (appUser == null) return Unauthorized("User context not found");

            var review = await _reviewRepo.GetReviewByUserAndGameAsync(appUser.Id, gameId);
            if (review == null) return NotFound("You have not reviewed this game");

            var updatedReview = await _reviewRepo.UpdateReviewAsync(review, updateDto);
            return Ok(updatedReview.ToReviewDto());
        }

        [HttpDelete("{gameId:int}")]
        public async Task<IActionResult> DeleteReview([FromRoute] int gameId)
        {
            var username = User.GetUserName();
            if (string.IsNullOrWhiteSpace(username)) return Unauthorized("Could not extract username from token claims");

            var appUser = await _userManager.FindByNameAsync(username);
            if (appUser == null) return Unauthorized("User context not found");

            var review = await _reviewRepo.GetReviewByUserAndGameAsync(appUser.Id, gameId);
            if (review == null) return NotFound("Review does not exist");

            await _reviewRepo.DeleteReviewAsync(review.Id);
            return NoContent();
        }
    }
}