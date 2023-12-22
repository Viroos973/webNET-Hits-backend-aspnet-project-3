using System.ComponentModel.DataAnnotations;

namespace MIS_Backend.DTO
{
    public class SpecialtiesPagedListModel
    {
        [Required]
        public List<SpecialityModel> Specialties { get; set; }

        [Required]
        public PageInfoModel Pagination { get; set; }
    }
}
