using System.ComponentModel.DataAnnotations;

namespace MIS_Backend.DTO
{
    public class TokenResponseModel
    {
        [Required]
        [MinLength(1)]
        public string Token { get; set; }
    }
}
