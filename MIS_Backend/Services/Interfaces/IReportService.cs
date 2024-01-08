using MIS_Backend.DTO;

namespace MIS_Backend.Services.Interfaces
{
    public interface IReportService
    {
        Task<IcdRootsReportModel> GetReport(DateTime start, DateTime end, List<Guid> icdRoots);
    }
}
