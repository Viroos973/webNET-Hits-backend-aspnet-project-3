using System.ComponentModel.DataAnnotations;

namespace MIS_Backend.DTO
{
    public class IcdRootsReportModel
    {
        [Required]
        public IcdRootsReportFiltersModel Filters { get; set; }

        public List<IcdRootsReportRecordModel> Records { get; set; }

        public Dictionary<string, int> SummaryByRoot { get; set; }
    }
}
