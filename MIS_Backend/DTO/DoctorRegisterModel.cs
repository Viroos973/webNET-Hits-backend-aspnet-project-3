using System.ComponentModel.DataAnnotations;
using MIS_Backend.Database.Enums;

namespace MIS_Backend.DTO
{
    public class DoctorRegisterModel
    {
        [MinLength(1)]
        [Required]
        public string Name { get; set; }

        [Required]
        [MinLength(1)]
        public string Password { get; set; }

        [Required]
        [MinLength(1)]
        [EmailAddress]
        public string EmailAddress { get; set; }

        public DateTime? BirthDate { get; set; }

        [Required]
        public Gender Genders { get; set; }

        [Phone]
        public string? Phone { get; set; }

        [Required]
        public Guid Speciality { get; set; }
    }
}
