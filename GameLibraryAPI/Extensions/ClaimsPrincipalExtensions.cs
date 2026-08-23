using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace GameLibraryAPI.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static string GetUserName(this ClaimsPrincipal user)
        {
            var username = user.Identity?.Name;

            if (string.IsNullOrWhiteSpace(username))
                throw new UnauthorizedAccessException("Username claim is missing from the token");

            return username;
        }
    }
}