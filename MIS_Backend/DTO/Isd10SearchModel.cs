using System.ComponentModel.DataAnnotations;

namespace MIS_Backend.DTO
{
    public class Isd10SearchModel
    {
        [Required]
        public List<Isd10RecordModel> records { get; set; }

        [Required]
        public PageInfoModel Pagination { get; set; }
    }
}
