using AutoMapper;
using MIS_Backend.Database.Models;
using MIS_Backend.DTO;

namespace MIS_Backend.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile() {
            CreateMap<Specialyti, SpecialityModel>();
            CreateMap<MedicalRecord, Isd10RecordModel>()
                .ForMember(
                dest => dest.Code,
                opt => opt.MapFrom(src => src.MkbCode)
                )
                .ForMember(
                dest => dest.Name,
                opt => opt.MapFrom(src => src.MkbName)
                )
                .ForMember(
                dest => dest.Id,
                opt => opt.MapFrom(src => src.Id)
                )
                .ForMember(
                dest => dest.CreateTime,
                opt => opt.MapFrom(src => src.Createtime)
                );
            CreateMap<Doctor, DoctorModel>();
        }
    }
}
