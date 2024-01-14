using MIS_Backend.DTO;

namespace MIS_Backend.Services.Interfaces
{
    public interface IInspectionService
    {
        Task<InspectionModel> GetSpecificInspection(Guid inspectionId);
        Task EdiInspection(InspectionEditModel inspectionEdit, Guid inspectionId, Guid doctorId);
        Task<List<InspectionPreviewModel>> GetInspectionChain(Guid inspectionId);
    }
}
