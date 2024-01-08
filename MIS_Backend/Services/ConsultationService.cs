using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MIS_Backend.Database;
using MIS_Backend.Database.Enums;
using MIS_Backend.Database.Models;
using MIS_Backend.DTO;
using MIS_Backend.Services.Interfaces;
using System.Security;

namespace MIS_Backend.Services
{
    public class ConsultationService : IConsultationService
    {
        private readonly AppDbContext _context;
        private readonly Isd10Context _isd10Context;
        private readonly IMapper _mapper;

        public ConsultationService(AppDbContext context, Isd10Context isd10Context, IMapper mapper)
        {
            _context = context;
            _isd10Context = isd10Context;
            _mapper = mapper;
        }

        public async Task<InspectionPagedListModel> GetInspectionForConsultation(Guid doctorId, bool? grouped, List<Guid> icdRoots, int? page, int? size)
        {
            if (size <= 0)
            {
                throw new BadHttpRequestException(message: $"Size value must be greater than 0");
            }

            var doctor = _context.Doctors.Where(x => x.Id == doctorId).FirstOrDefault();

            if (doctor == null)
            {
                throw new KeyNotFoundException(message: $"Doctor not found");
            }

            var isd10 = _isd10Context.MedicalRecords.Where(x => x.IdParent == null).Select(x => x.Id);

            foreach (var icd in icdRoots)
            {
                if (!isd10.Contains(icd))
                {
                    throw new BadHttpRequestException(message: $"isd10 root with id={icd} not found");
                }
            }

            var inspectionWithoutISD = await _context.Inspections
                .Include(x => x.Consultations)
                .Where(x => x.Consultations.Count > 0 && x.Consultations.Any(c => c.SpecialityId == doctor.Speciality))
                .Include(x => x.Diagnoses)
                .Include(x => x.Patients)
                .Include(x => x.Doctors).ToListAsync();

            if (grouped == true)
            {
                inspectionWithoutISD = inspectionWithoutISD.Where(x => x.PreviousInspectionId == null).ToList();
            }

            List<InspectionPreviewModel> inspections = new List<InspectionPreviewModel>();

            for (int i = 0; i < inspectionWithoutISD.Count; i++)
            {
                var diagnosis = inspectionWithoutISD[i].Diagnoses.Where(x => x.Type == DiagnosisType.Main.ToString())
                    .Join(_isd10Context.MedicalRecords,
                      d => d.IcdDiagnosisId,
                      m => m.Id,
                      (d, m) => new
                      {
                          Id = d.Id,
                          CreateTime = d.CreateTime,
                          Code = m.MkbCode,
                          Name = m.MkbName,
                          Discription = d.Discription,
                          Type = d.Type,
                          Root = m.Root
                      }).First();

                if (icdRoots.Count == 0 || icdRoots.Contains(diagnosis.Root))
                    inspections.Add(new InspectionPreviewModel
                    {
                        Id = inspectionWithoutISD[i].Id,
                        CreateTime = inspectionWithoutISD[i].CreateTime,
                        PreviousId = inspectionWithoutISD[i].PreviousInspectionId,
                        Date = inspectionWithoutISD[i].Date,
                        Conclusion = inspectionWithoutISD[i].Conclusion,
                        PatientId = inspectionWithoutISD[i].PatientId,
                        Patient = inspectionWithoutISD[i].Patients.Name,
                        DoctorId = inspectionWithoutISD[i].DoctorId,
                        Doctor = inspectionWithoutISD[i].Doctors.Name,
                        Diagnosis = new DiagnosisModel
                        {
                            Id = diagnosis.Id,
                            CreateTime = diagnosis.CreateTime,
                            Code = diagnosis.Code,
                            Name = diagnosis.Name,
                            Discription = diagnosis.Discription,
                            Type = diagnosis.Type
                        },
                        HasChain = inspectionWithoutISD[i].HasChain,
                        HasNested = inspectionWithoutISD[i].HasNested
                    });
            }

            var maxPage = (int)Math.Ceiling(inspections.Count() / (double)size);

            if ((page < 1 || inspections.Count() <= (page - 1) * size) && maxPage > 0)
            {
                throw new BadHttpRequestException(message: $"Page value must be greater than 0 and less than {maxPage + 1}");
            }

            inspections = inspections.OrderByDescending(x => x.Date).Skip((int)((page - 1) * size)).Take((int)size).ToList();

            var pagination = new PageInfoModel
            {
                Size = (int)size,
                Count = maxPage,
                Current = (int)page
            };

            return new InspectionPagedListModel
            {
                Inspections = inspections,
                Pagination = pagination
            };
        }

        public async Task<ConsultationModel> GetConsultation(Guid consultationId)
        {
            var consultation = _context.Consultations.Where(x => x.Id == consultationId)
                .Include(x => x.Comments).ThenInclude(x => x.Doctors)
                .Include(x => x.Specialytis).FirstOrDefault();

            if (consultation == null)
            {
                throw new KeyNotFoundException(message: $"Consultation with id={consultationId} not found in database");
            }

            return _mapper.Map<ConsultationModel>(consultation);
        }

        public async Task<Guid> AddComment(Guid consultationId, CommentCreateModel comment, Guid doctorId)
        {
            var consultation = _context.Consultations.Where(x => x.Id == consultationId).Include(x => x.Inspections).FirstOrDefault();

            if (consultation == null)
            {
                throw new KeyNotFoundException(message: $"Consultation with id={consultationId} not found in database");
            }

            var doctor = _context.Doctors.Where(x => x.Id == doctorId).FirstOrDefault();

            if (doctor == null)
            {
                throw new KeyNotFoundException(message: $"Doctor with id={doctorId} not found in database");
            }

            if (consultation.SpecialityId != doctor.Speciality && consultation.Inspections.DoctorId != doctorId)
            {
                throw new SecurityException(message: "The user has an inappropriate specialty and is not the author of the inspection");
            }

            var commentParent = _context.Comments.Where(x => x.Id == comment.ParentId).FirstOrDefault();

            if (commentParent == null)
            {
                throw new KeyNotFoundException(message: $"Comment with id={comment.ParentId} not found in database");
            }

            if (commentParent.CosultationId != consultationId)
            {
                throw new BadHttpRequestException(message: $"Incorrect combination between consultation with id={consultationId} and parent comment with id={comment.ParentId}");
            }

            var commentId = Guid.NewGuid();

            await _context.Comments.AddAsync(new Comment
            {
                Id = commentId,
                CreateTime = DateTime.UtcNow,
                ModifiedDate = null,
                Content = comment.Content,
                Author = doctorId,
                ParentId = comment.ParentId,
                CosultationId = consultationId
            });
            await _context.SaveChangesAsync();

            return commentId;
        }

        public async Task EditComment(Guid idComment, InspectionCommentCreateModel commentEdit, Guid doctorId)
        {
            var comment = _context.Comments.Where(x => x.Id == idComment).FirstOrDefault();

            if (comment == null)
            {
                throw new KeyNotFoundException(message: $"Comment with id={idComment} not found in database");
            }

            var doctor = _context.Doctors.Where(x => x.Id == doctorId).FirstOrDefault();

            if (doctor == null)
            {
                throw new KeyNotFoundException(message: $"Doctor with id={doctorId} not found in database");
            }

            if (comment.Author != doctorId)
            {
                throw new SecurityException(message: "The user is not the author of the comment");
            }

            comment.Content = commentEdit.Content;
            comment.ModifiedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }
    }
}
