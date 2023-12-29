using System.ComponentModel.DataAnnotations;

namespace MIS_Backend.DTO
{
    public class PatientPagedListModel
    {
        [Required]
        public List<PatientModel> Patients { get; set; }

        [Required]
        public PageInfoModel Pagination { get; set; }
    }
}
