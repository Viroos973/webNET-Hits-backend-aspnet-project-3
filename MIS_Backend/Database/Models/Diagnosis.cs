using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MIS_Backend.Database.Models
{
    public class Diagnosis
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public DateTime CreateTime { get; set; }

        [Required]
        public Guid IcdDiagnosisId { get; set; }

        [MaxLength(5000)]
        public string? Discription { get; set; }

        [Required]
        public string Type { get; set; }

        [Required]
        public Guid InspectionId { get; set; }
        [ForeignKey("InspectionId")]
        public Inspection Inspections { get; set; }
    }
}
