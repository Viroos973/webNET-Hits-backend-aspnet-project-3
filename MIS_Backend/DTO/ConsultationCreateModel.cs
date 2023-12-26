using System.ComponentModel.DataAnnotations;

namespace MIS_Backend.DTO
{
    public class ConsultationCreateModel
    {
        [Required]
        public Guid SpecialityId { get; set; }

        [Required]
        public InspectionCommentCreateModel comment { get; set; }
    }
}
