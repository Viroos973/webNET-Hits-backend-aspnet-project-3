using System.ComponentModel.DataAnnotations;

namespace MIS_Backend.Database.Models
{
    public class Token
    {
        [Key]
        [Required]
        public string InvalideToken { get; set; }

        [Required]
        public DateTime ExpiredDate { get; set; }
    }
}
