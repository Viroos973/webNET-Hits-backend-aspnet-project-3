using System.ComponentModel.DataAnnotations;

namespace MIS_Backend.DTO
{
    public class DiagnosisModel
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        public DateTime CreateTime { get; set; }

        [Required]
        public string Code { get; set; }

        [Required]
        public string Name { get; set; }

        public string? Discription { get; set; }

        [Required]
        public string Type { get; set; }
    }
}
