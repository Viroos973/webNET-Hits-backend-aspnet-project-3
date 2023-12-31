using MIS_Backend.Database.Enums;
using MIS_Backend.Database.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MIS_Backend.DTO
{
    public class InspectionPreviewModel
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        public DateTime CreateTime { get; set; }

        public Guid? PreviousId { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        public Conclusion Conclusion { get; set; }

        [Required]
        public Guid PatientId { get; set; }

        [Required]
        public string Patient { get; set; }

        [Required]
        public Guid DoctorId { get; set; }

        [Required]
        public string Doctor { get; set; }

        [Required]
        public List<DiagnosisModel> Diagnosis { get; set; }

        public bool? HasChain { get; set; }

        public bool? HasNested { get; set; }
    }
}
