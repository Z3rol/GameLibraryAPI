using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace GameLibraryAPI.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static string GetUserName(this ClaimsPrincipal user)
        {
            var username = user.Identity?.Name ?? user.FindFirstValue(JwtRegisteredClaimNames.GivenName);

            if (string.IsNullOrWhiteSpace(username))
                throw new UnauthorizedAccessException("Username claim is missing from the token");

            return username;
        }

        public static string GetUserId(this ClaimsPrincipal user)
        {
            return user.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? user.FindFirstValue(JwtRegisteredClaimNames.Sub)
                ?? throw new UnauthorizedAccessException("User ID claim is missing from the token");
        }
    }
}