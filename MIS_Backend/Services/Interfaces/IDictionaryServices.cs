using MIS_Backend.DTO;

namespace MIS_Backend.Services.Interfaces
{
    public interface IDictionaryServices
    {
        Task<SpecialtiesPagedListModel> GetSpecialytis(string? name, int? page, int? size);
    }
}
