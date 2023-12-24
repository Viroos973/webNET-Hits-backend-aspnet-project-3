using System.ComponentModel.DataAnnotations;

namespace MIS_Backend.DTO
{
    public class LoginCredentialsModel
    {
        [Required]
        [MinLength(1)]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [MinLength(1)]
        public string Password { get; set; }
    }
}
