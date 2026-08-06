using System.ComponentModel.DataAnnotations;

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