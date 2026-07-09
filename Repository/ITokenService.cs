using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameLibraryAPI.Models;

namespace GameLibraryAPI.Repository
{
    public interface ITokenService
    {
        string CreateToken(AppUser user);
    }
}