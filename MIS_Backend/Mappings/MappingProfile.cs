using AutoMapper;
using MIS_Backend.Database.Enums;
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
            CreateMap<Consultation, InspectionConsultationModel>()
                .ForMember(
                dest => dest.Speciality,
                opt => opt.MapFrom(src => new SpecialityModel
                {
                    Id = src.Specialytis.Id,
                    CreateTime = src.Specialytis.CreateTime,
                    Name = src.Specialytis.Name,
                }))
                .ForMember(
                dest => dest.RootComment,
                opt => opt.MapFrom(src => src.Comments.Where(x => x.ParentId == null)
                .Select(c => new InspectionCommentModel
                {
                    Id = c.Id,
                    CreateTime = c.CreateTime,
                    ParentId = c.ParentId,
                    Content = c.Content,
                    Author = new DoctorModel
                    {
                        Id = c.Doctors.Id,
                        CreateTime = c.Doctors.CreateTime,
                        Name = c.Doctors.Name,
                        BirthDate = c.Doctors.BirthDate,
                        Genders = (Gender)Enum.Parse(typeof(Gender), c.Doctors.Genders),
                        EmailAddress = c.Doctors.EmailAddress,
                        Phone = c.Doctors.Phone
                    },
                    ModifiedDate = c.ModifiedDate
                }).FirstOrDefault()))
                .ForMember(
                dest => dest.CommentsNumber,
                opt => opt.MapFrom(src => src.Comments.Count));
            CreateMap<Consultation, ConsultationModel>().ForMember(
                dest => dest.Speciality,
                opt => opt.MapFrom(src => new SpecialityModel
                {
                    Id = src.Specialytis.Id,
                    CreateTime = src.Specialytis.CreateTime,
                    Name = src.Specialytis.Name,
                }))
                .ForMember(
                dest => dest.Comments,
                opt => opt.MapFrom(src => src.Comments.Select(c => new CommentModel
                { 
                    Id = c.Id,
                    CreateTime = c.CreateTime,
                    ModifiedDate = c.ModifiedDate,
                    Content = c.Content,
                    AuthorId = c.Author,
                    Author = c.Doctors.Name,
                    ParentId = c.ParentId,
                }).OrderBy(x => x.CreateTime).ToList()));
        }
    }
}
