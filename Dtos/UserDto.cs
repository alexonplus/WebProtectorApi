using System.ComponentModel.DataAnnotations;

namespace WebProtectorApi.Dtos
{
    public class UserRegisterDto
    {
        [Required]
        [StringLength(20, MinimumLength = 3)]
        public string Username { get; set; } = string.Empty;

        [Required]
        [StringLength(100, MinimumLength = 8)] // minimum 8 characters for better security
        public string Password { get; set; } = string.Empty;

       
    }

    public class UserLoginDto
    {
        [Required]
        public string Username { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
    }
}