using MIS_Backend.Database.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MIS_Backend.DTO
{
    public class InspectionConsultationModel
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
        public InspectionCommentModel RootComment { get; set; }

        [Required]
        public int CommentsNumber { get; set; }
    }
}
