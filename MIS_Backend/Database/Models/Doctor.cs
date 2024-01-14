using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MIS_Backend.Database.Models
{
    public class Doctor
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public DateTime CreateTime { get; set; }

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
        public string Genders { get; set; }

        [Phone]
        public string? Phone { get; set; }

        [Required]
        public Guid? Speciality { get; set; }
        [ForeignKey("Speciality")]
        public Specialyti Specialytis { get; set; }

        public ICollection<Inspection> Inspections { get; set; }

        public ICollection<Comment> Comments { get; set; }
    }
}
