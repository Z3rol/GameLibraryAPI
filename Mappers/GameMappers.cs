using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameLibraryAPI.DTOs.Game;
using GameLibraryAPI.Models;

namespace GameLibraryAPI.Mappers
{
    public static class GameMappers
    {
        public static GameDto ToGameDto(this Game game)
        {
            return new GameDto
            {
                Id = game.Id,
                Name = game.Name,
                Genre = game.Genre,
                DeveloperName = game.DeveloperName,
                Price = game.Price,
                ReleasedOn = game.ReleasedOn
            };
        }

        public static Game ToGameFromCreate(this CreateGameRequestDto gameDto)
        {
            return new Game
            {
                Name = gameDto.Name,
                Genre = gameDto.Genre,
                DeveloperName = gameDto.DeveloperName,
                Price = gameDto.Price,
                ReleasedOn = gameDto.ReleasedOn
            };
        }
    }
}