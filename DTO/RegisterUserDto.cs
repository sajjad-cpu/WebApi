using System.ComponentModel.DataAnnotations;

namespace Pinjet.DTO
{
    public class RegisterUserDto
    {
        [Required]
        [MaxLength]
        public string Username { get; set; }

        [Required]
        [EmailAddress]
        [MaxLength(100)]
        public string Email { get; set; }

        [Required]
        [MinLength(6)]
        public string Password { get; set; }
    }
}
