using System.ComponentModel.DataAnnotations;

namespace MIS_Backend.DTO
{
    public class InspectionPagedListModel
    {
        [Required]
        public List<InspectionPreviewModel> Inspections { get; set; }

        [Required]
        public PageInfoModel Pagination { get; set; }
    }
}
