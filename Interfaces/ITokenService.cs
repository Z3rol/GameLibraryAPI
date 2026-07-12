using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameLibraryAPI.Models;

namespace GameLibraryAPI.Interfaces
{
    public interface ITokenService
    {
        string CreateToken(AppUser user, IList<string> roles);
    }
}