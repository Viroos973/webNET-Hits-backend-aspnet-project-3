using MIS_Backend.DTO;

namespace MIS_Backend.Services.Interfaces
{
    public interface IDoctorService
    {
        Task<TokenResponseModel> RegisterUser(DoctorRegisterModel userRegisterModel);
        Task<TokenResponseModel> Login(LoginCredentialsModel credentials);
        Task LogOut(string token);
        Task<DoctorModel> GetProfile(Guid id);
        Task EditProfile(DoctorEditModel editModel, Guid id);
    }
}
