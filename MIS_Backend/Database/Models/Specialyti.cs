using System.ComponentModel.DataAnnotations;

namespace MIS_Backend.Database.Models
{
    public class Specialyti
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public DateTime CreateTime { get; set; }

        [MinLength(1)]
        [Required]
        public string Name { get; set; }

        public ICollection<Doctor> Doctors { get; set; }
    }
}
