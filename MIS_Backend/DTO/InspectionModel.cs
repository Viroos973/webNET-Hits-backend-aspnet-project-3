using MIS_Backend.Database.Enums;
using MIS_Backend.Database.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MIS_Backend.DTO
{
    public class InspectionModel
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        public DateTime CreateTime { get; set; }

        [Required]
        public DateTime Date { get; set; }

        public string? Anamnesis { get; set; }

        public string? Complaints { get; set; }

        public string? Treatment { get; set; }

        [Required]
        public Conclusion Conclusion { get; set; }

        public DateTime? NextVisitDate { get; set; }

        public DateTime? DeathDate { get; set; }

        public Guid? BaseInspectionId { get; set; }

        public Guid? PreviousInspectionId { get; set; }

        [Required]
        public PatientModel Patient { get; set; }

        [Required]
        public DoctorModel Doctor { get; set; }

        public List<DiagnosisModel> Diagnoses { get; set; }

        public List<InspectionConsultationModel> Consultations { get; set; }
    }
}
