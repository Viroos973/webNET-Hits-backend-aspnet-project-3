using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MIS_Backend.Database.Models
{
    public class Consultation
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public DateTime CreateTime { get; set; }

        [Required]
        public Guid InspectionId { get; set; }
        [ForeignKey("InspectionId")]
        public Inspection Inspections { get; set; }

        [Required]
        public Guid SpecialityId { get; set; }
        [ForeignKey("SpecialityId")]
        public Specialyti Specialytis { get; set; }

        public ICollection<Comment> Comments { get; set; }
    }
}
