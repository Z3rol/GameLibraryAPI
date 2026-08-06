using GameLibraryAPI.DTOs.Game;
using GameLibraryAPI.Helpers;
using GameLibraryAPI.Models;

namespace GameLibraryAPI.Interfaces
{
    public interface ILibraryRepository
    {
        public Task<List<GameDto>> GetUserLibraryAsync(string userId, LibraryQueryObject query);
        public Task<bool> UserOwnsGameAsync(string userId, int gameId);
        public Task<UserGame> AddGameToLibraryAsync(string userId, int gameId);
        public Task<UserGame?> RemoveGameFromLibraryAsync(string userId, int gameId);
    }
}