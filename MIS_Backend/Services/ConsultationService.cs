using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MIS_Backend.Database;
using MIS_Backend.Database.Enums;
using MIS_Backend.DTO;
using MIS_Backend.Services.Interfaces;

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

            var maxPage = (int)((inspections.Count() + size - 1) / size);

            if ((page < 1 || inspections.Count() <= (page - 1) * size) && maxPage > 0)
            {
                throw new BadHttpRequestException(message: $"Page value must be greater than 0 and less than {maxPage + 1}");
            }

            inspections = inspections.Skip((int)((page - 1) * size)).Take((int)size).ToList();

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
    }
}
