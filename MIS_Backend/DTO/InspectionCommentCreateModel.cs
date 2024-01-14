using System.ComponentModel.DataAnnotations;

namespace MIS_Backend.DTO
{
    public class InspectionCommentCreateModel
    {
        [Required]
        [MinLength(1)]
        [MaxLength(1000)]
        public string Content { get; set; }
    }
}
