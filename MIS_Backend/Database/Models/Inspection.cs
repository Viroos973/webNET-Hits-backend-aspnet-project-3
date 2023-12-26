using MIS_Backend.Database.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MIS_Backend.Database.Models
{
    public class Inspection
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public DateTime CreateTime { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        [MinLength(1)]
        [MaxLength(5000)]
        public string Anamnesis { get; set; }

        [Required]
        [MinLength(1)]
        [MaxLength(5000)]
        public string Complaints { get; set; }

        [Required]
        [MinLength(1)]
        [MaxLength(5000)]
        public string Treatment { get; set; }

        [Required]
        public Conclusion Conclusion { get; set; }

        public DateTime? NextVisitDate { get; set; }

        public DateTime? DeathDate { get; set; }

        public Guid? BaseInspectionId { get; set; }

        public Guid? PreviousInspectionId { get; set; }

        [Required]
        public Guid PatientId { get; set; }
        [ForeignKey("PatientId")]
        public Patient Patients { get; set; }

        [Required]
        public Guid DoctorId { get; set; }
        [ForeignKey("DoctorId")]
        public Doctor Doctors { get; set; }

        [Required]
        public bool HasChain { get; set; }

        [Required]
        public bool HasNested { get; set; }

        public ICollection<Consultation> Consultations { get; set; }

        public ICollection<Diagnosis> Diagnoses { get; set; }
    }
}
