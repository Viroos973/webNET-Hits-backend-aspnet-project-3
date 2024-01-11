using System.ComponentModel.DataAnnotations;

namespace MIS_Backend.DTO
{
    public class ConsultationModel
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        public DateTime CreateTime { get; set; }

        [Required]
        public Guid InspectionId { get; set; }

        [Required]
        public SpecialityModel Speciality { get; set; }

        [Required]
        public List<CommentModel> Comments { get; set; }
    }
}
