using MIS_Backend.Database.Enums;
using System.ComponentModel.DataAnnotations;

namespace MIS_Backend.DTO
{
    public class DiagnosisCreateModel
    {
        [Required]
        public Guid IcdDiagnosisId { get; set; }

        [MaxLength(5000)]
        public string? Discription { get; set; }

        [Required]
        public DiagnosisType Type { get; set; }
    }
}
