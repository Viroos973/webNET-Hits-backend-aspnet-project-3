using System.ComponentModel.DataAnnotations;

namespace MIS_Backend.DTO
{
    public class SpecialityModel
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        public DateTime CreateTime { get; set; }

        [MinLength(1)]
        [Required]
        public string Name { get; set; }
    }
}
