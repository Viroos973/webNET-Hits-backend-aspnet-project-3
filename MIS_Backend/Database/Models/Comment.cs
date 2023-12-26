using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MIS_Backend.Database.Models
{
    public class Comment
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public DateTime CreateTime { get; set; }

        public DateTime? ModifiedDate { get; set; }

        [Required]
        [MinLength(1)]
        public string Content { get; set; }

        [Required]
        public Guid Author { get; set; }
        [ForeignKey("Author")]
        public Doctor Doctors { get; set; }

        public Guid? ParentId { get; set; }

        [Required]
        public Guid CosultationId { get; set; }
        [ForeignKey("CosultationId")]
        public Consultation Consultations { get; set; }
    }
}
