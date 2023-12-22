using System.ComponentModel.DataAnnotations;

namespace MIS_Backend.DTO
{
    public class Isd10RecordModel
    {
        public string Code { get; set; }

        public string Name { get; set; }

        public int Id { get; set; }

        public DateOnly? CreateTime { get; set; }
    }
}
