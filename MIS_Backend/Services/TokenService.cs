using MIS_Backend.Database;
using MIS_Backend.Services.Interfaces;

namespace MIS_Backend.Services
{
    public class TokenService : ITokenService
    {
        private readonly AppDbContext _context;

        public TokenService(AppDbContext context)
        {
            _context = context;
        }

        public async Task CheckToken(string token)
        {
            var invalidToken = _context.Tokens.Where(x => x.InvalideToken == token).FirstOrDefault();

            if (invalidToken != null)
            {
                throw new UnauthorizedAccessException();
            }
        }
    }
}
