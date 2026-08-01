using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GameLibraryAPI.DTOs.Account
{
    public class LoginDto
    {
        [Required(ErrorMessage = "Username or Email is required.")]
        public string UserNameOrEmail { get; set; } = "";
        [Required(ErrorMessage = "Password is required.")]
        public string Password { get; set; } = "";
    }
}