using MIS_Backend.Database.Enums;
using System.ComponentModel.DataAnnotations;

namespace MIS_Backend.DTO
{
    public class PatientModel
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        public DateTime CreateTime { get; set; }

        [MinLength(1)]
        [Required]
        public string Name { get; set; }

        public DateTime? BirthDate { get; set; }

        [Required]
        public Gender Genders { get; set; }
    }
}
