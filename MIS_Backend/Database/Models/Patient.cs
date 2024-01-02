using System.ComponentModel.DataAnnotations;

namespace MIS_Backend.Database.Models
{
    public class Patient
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public DateTime CreateTime { get; set; }

        [MinLength(1)]
        [MaxLength(1000)]
        [Required]
        public string Name { get; set; }

        public DateTime? BirthDate { get; set; }

        [Required]
        public string Genders { get; set; }

        public ICollection<Inspection> Inspections { get; set; }
    }
}
