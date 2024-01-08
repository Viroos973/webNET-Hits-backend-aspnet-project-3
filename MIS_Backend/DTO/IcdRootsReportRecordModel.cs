using System.ComponentModel.DataAnnotations;

namespace MIS_Backend.DTO
{
    public class IcdRootsReportRecordModel
    {
        public string? PatientName { get; set; }

        public DateTime? PatientBirthDate { get; set; }

        [Required]
        public string Gender { get; set; }

        public Dictionary<string, int> VisitByRoots { get; set; }
    }
}
