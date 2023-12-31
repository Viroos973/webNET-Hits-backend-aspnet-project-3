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
                );
            CreateMap<Doctor, DoctorModel>();
            CreateMap<Patient, PatientModel>();
        }
    }
}
