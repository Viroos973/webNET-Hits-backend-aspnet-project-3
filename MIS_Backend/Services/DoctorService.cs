using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MIS_Backend.Database;
using MIS_Backend.Database.Models;
using MIS_Backend.DTO;
using MIS_Backend.Services.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace MIS_Backend.Services
{
    public class DoctorService : IDoctorService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public DoctorService(AppDbContext context, IMapper mapper)
        {
            _mapper = mapper;
            _context = context;
        }

        public async Task<TokenResponseModel> RegisterUser(DoctorRegisterModel doctorRegisterModel)
        {
            if (doctorRegisterModel.BirthDate != null && doctorRegisterModel.BirthDate > DateTime.UtcNow)
            {
                throw new BadHttpRequestException(message: "Birth date can't be later than today");
            }

            var email = _context.Doctors.Where(x => x.EmailAddress == doctorRegisterModel.EmailAddress).FirstOrDefault();

            if (email != null)
            {
                throw new BadHttpRequestException(message: $"Username {doctorRegisterModel.EmailAddress} is already taken");
            }

            var address = _context.Specialytis.Where(x => x.Id == doctorRegisterModel.Speciality).FirstOrDefault();

            if (address == null)
            {
                throw new BadHttpRequestException(message: "Speciality not found");
            }

            if (!doctorRegisterModel.Password.Any(char.IsDigit))
            {
                throw new BadHttpRequestException(message: "Password requires at least one digit");
            }

            var passwordHash = await PasswordHashing(doctorRegisterModel.Password);

            await _context.Doctors.AddAsync(new Doctor
            {
                Id = Guid.NewGuid(),
                CreateTime = DateTime.UtcNow,
                Name = doctorRegisterModel.Name,
                BirthDate = doctorRegisterModel.BirthDate,
                Speciality = doctorRegisterModel.Speciality,
                EmailAddress = doctorRegisterModel.EmailAddress,
                Genders = doctorRegisterModel.Genders.ToString(),
                Password = passwordHash,
                Phone = doctorRegisterModel.Phone
            });
            await _context.SaveChangesAsync();

            var credentials = new LoginCredentialsModel
            {
                Email = doctorRegisterModel.EmailAddress,
                Password = doctorRegisterModel.Password
            };

            return await Login(credentials);
        }

        public async Task<TokenResponseModel> Login(LoginCredentialsModel credentials)
        {
            var userEntity = _context.Doctors.Where(x => x.EmailAddress == credentials.Email).FirstOrDefault();

            if (userEntity == null)
            {
                throw new BadHttpRequestException(message: "User is not found");
            }

            var passwordHash = await PasswordHashing(credentials.Password);

            if (userEntity.Password != passwordHash)
            {
                throw new BadHttpRequestException(message: "Wrong password");
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes("jAoRAj9kzVPykFATzV1Ye0LJNmdcuB");

            var TokenDescriptor = new SecurityTokenDescriptor()
            {
                NotBefore = DateTime.UtcNow,
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Issuer = "MISBackend",
                Audience = "MISFronted",
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, userEntity.Id.ToString()),
                })
            };

            return new TokenResponseModel
            {
                Token = tokenHandler.WriteToken(tokenHandler.CreateToken(TokenDescriptor))
            };
        }

        public async Task LogOut(string token)
        {
            string invalideToken = token;
            DateTime expires = new JwtSecurityTokenHandler().ReadJwtToken(token).ValidTo;

            await _context.Tokens.AddAsync(new Token
            {
                InvalideToken = invalideToken,
                ExpiredDate = expires
            });

            await _context.SaveChangesAsync();
        }

        public async Task<DoctorModel> GetProfile(Guid id)
        {
            var user = await _context.Doctors.Where(x => x.Id == id).FirstOrDefaultAsync();

            if (user == null)
            {
                throw new KeyNotFoundException(message: $"User with id={id} don't in database");
            }

            return _mapper.Map<DoctorModel>(user);
        }

        public async Task EditProfile(DoctorEditModel editModel, Guid id)
        {
            if (editModel.BirthDate != null && editModel.BirthDate > DateTime.UtcNow)
            {
                throw new BadHttpRequestException(message: "Birth date can't be later than today");
            }

            var email = _context.Doctors.Where(x => x.EmailAddress == editModel.EmailAddress).FirstOrDefault();

            if (email != null)
            {
                throw new BadHttpRequestException(message: $"Username {editModel.EmailAddress} is already taken");
            }

            var doctor = await _context.Doctors.Where(x => x.Id == id).FirstOrDefaultAsync();

            doctor.EmailAddress = editModel.EmailAddress;
            doctor.Name = editModel.Name;
            doctor.BirthDate = editModel.BirthDate;
            doctor.Genders = editModel.Genders.ToString();
            doctor.Phone = editModel.Phone;

            await _context.SaveChangesAsync();
        }

        private async Task<string> PasswordHashing(string password)
        {
            var md5 = MD5.Create();
            var hash = md5.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hash);
        }
    }
}
