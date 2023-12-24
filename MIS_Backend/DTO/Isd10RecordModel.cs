using System.ComponentModel.DataAnnotations;

namespace MIS_Backend.DTO
{
    public class Isd10RecordModel
    {
        public string Code { get; set; }

        public string Name { get; set; }

        public Guid Id { get; set; }

        public DateTime? CreateTime { get; set; }
    }
}
