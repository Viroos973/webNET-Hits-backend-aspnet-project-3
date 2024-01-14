using System.Text.Json.Serialization;

namespace MIS_Backend.Database.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum PatientSorting
    {
        NameAsc,
        NameDesc,
        CreateAsc,
        CreateDesc,
        InspectionAsc,
        InspectionDesc
    }
}
