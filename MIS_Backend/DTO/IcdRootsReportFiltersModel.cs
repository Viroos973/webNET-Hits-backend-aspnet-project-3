using System.ComponentModel.DataAnnotations;

namespace MIS_Backend.DTO
{
    public class IcdRootsReportFiltersModel
    {
        [Required]
        public DateTime Start { get; set; }

        [Required]
        public DateTime End { get; set; }

        public List<string> IsdRoots { get; set; }
    }
}
