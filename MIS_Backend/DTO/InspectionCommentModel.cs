using System.ComponentModel.DataAnnotations;

namespace MIS_Backend.DTO
{
    public class InspectionCommentModel
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        public DateTime CreateTime { get; set; }

        public Guid? ParentId { get; set; }

        public string? Content { get; set; }

        [Required]
        public DoctorModel Author { get; set; }

        public DateTime? ModifiedDate { get; set; }
    }
}
