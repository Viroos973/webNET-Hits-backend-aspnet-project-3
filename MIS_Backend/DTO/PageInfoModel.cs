using System.ComponentModel.DataAnnotations;

namespace MIS_Backend.DTO
{
    public class PageInfoModel
    {
        [Required]
        public int Size { get; set; }

        [Required]
        public int Count { get; set; }

        [Required]
        public int Current { get; set; }
    }
}
