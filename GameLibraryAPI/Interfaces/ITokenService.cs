using GameLibraryAPI.Models;

namespace GameLibraryAPI.Interfaces
{
    public interface ITokenService
    {
        string CreateToken(AppUser user, IList<string> roles);
    }
}