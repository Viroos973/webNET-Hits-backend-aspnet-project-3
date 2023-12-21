using AutoMapper;
using MIS_Backend.Database.Models;
using MIS_Backend.DTO;

namespace MIS_Backend.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile() {
            CreateMap<Specialyti, SpecialityModel>();
        }
    }
}
