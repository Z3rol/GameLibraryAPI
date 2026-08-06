using GameLibraryAPI.DTOs.Game;
using GameLibraryAPI.Helpers;
using GameLibraryAPI.Models;

namespace GameLibraryAPI.Interfaces
{
    public interface IGameRepository
    {
        public Task<List<GameDto>> GetAllAsync(GameQueryObject query);
        public Task<GameDto?> GetByIdAsync(int id);
        public Task<Game?> GetGameEntityByIdAsync(int id);
        public Task<Game> CreateAsync(Game gameModel);
        public Task<GameDto> UpdateDatailsAsync(Game game, UpdateGameDetailsDto updateDto);
        public Task<Game?> DeleteAsync(int id);
        public Task<bool> GameExistsAsync(string name);
        public Task<bool> GameExistsAsync(int id);
    }
}