namespace MIS_Backend.Services.Interfaces
{
    public interface ITokenService
    {
        Task CheckToken(string token);
    }
}
