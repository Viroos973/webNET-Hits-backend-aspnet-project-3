using MIS_Backend.Database.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using MIS_Backend.Database.Enums;

namespace MIS_Backend.DTO
{
    public class DoctorModel
    {
        public Guid Id { get; set; }

        [Required]
        public DateTime CreateTime { get; set; }

        [MinLength(1)]
        [Required]
        public string Name { get; set; }

        public DateTime? BirthDate { get; set; }

        [Required]
        public Gender Genders { get; set; }

        [Required]
        [MinLength(1)]
        [EmailAddress]
        public string EmailAddress { get; set; }

        [Phone]
        public string? Phone { get; set; }
    }
}
