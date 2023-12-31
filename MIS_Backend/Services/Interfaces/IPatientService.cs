using MIS_Backend.Database.Enums;
using MIS_Backend.DTO;

namespace MIS_Backend.Services.Interfaces
{
    public interface IPatientService
    {
        Task<Guid> CreatePatient(PatientCreateModel patient);
        Task<Guid> CreateInspection(InspectionCreateModel inspection, Guid patientId, Guid doctorId);
        Task<PatientPagedListModel> GetPatient(Guid doctorId, string? name, List<Conclusion> conclusions, PatientSorting? sorting, bool? scheduledVisits,
            bool? onlyMine, int? page, int? size);
        Task<InspectionPagedListModel> GetInspections(Guid patientId, bool? grouped, List<Guid> icdRoots, int? page, int? size);
    }
}
