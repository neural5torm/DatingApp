using System.ComponentModel.DataAnnotations;

namespace DatingAppAPI.Dtos
{
    public class UserRegistrationDto
    {
        [Required]
        public string Username { get; set; }

        [Required]
        [StringLength(32, MinimumLength = 8, ErrorMessage = "You must specify a password between 8 and 32 characters.")]
        public string Password { get; set; }
    }
}
