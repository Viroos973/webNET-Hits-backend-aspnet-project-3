using MIS_Backend.Database.Enums;
using System.ComponentModel.DataAnnotations;

namespace MIS_Backend.DTO
{
    public class PatientCreateModel
    {
        [MinLength(1)]
        [MaxLength(1000)]
        [Required]
        public string Name { get; set; }

        public DateTime? BirthDate { get; set; }

        [Required]
        public Gender Genders { get; set; }
    }
}
