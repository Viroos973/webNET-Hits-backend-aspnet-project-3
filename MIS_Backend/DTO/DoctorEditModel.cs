using System.ComponentModel.DataAnnotations;
using MIS_Backend.Database.Enums;

namespace MIS_Backend.DTO
{
    public class DoctorEditModel
    {
        [Required]
        [MinLength(1)]
        [EmailAddress]
        public string EmailAddress { get; set; }

        [MinLength(1)]
        [MaxLength(1000)]
        [Required]
        public string Name { get; set; }

        public DateTime? BirthDate { get; set; }

        [Required]
        public Gender Genders { get; set; }

        [Phone]
        public string? Phone { get; set; }
    }
}
