using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MIS_Backend.Database;
using MIS_Backend.Database.Enums;
using MIS_Backend.Database.Models;
using MIS_Backend.DTO;
using MIS_Backend.Services.Interfaces;

namespace MIS_Backend.Services
{
    public class PatientService : IPatientService
    {
        private readonly AppDbContext _context;
        private readonly Isd10Context _isd10Context;
        private readonly IMapper _mapper;

        public PatientService(AppDbContext context, Isd10Context isd10Context, IMapper mapper)
        {
            _context = context;
            _isd10Context = isd10Context;
            _mapper = mapper;
        }

        public async Task<Guid> CreatePatient(PatientCreateModel patient)
        {
            if (patient.BirthDate != null && patient.BirthDate > DateTime.UtcNow)
            {
                throw new BadHttpRequestException(message: "Birth date can't be later than today");
            }

            Guid patientId = Guid.NewGuid();

            await _context.Patients.AddAsync(new Patient
            {
                Id = patientId,
                CreateTime = DateTime.UtcNow,
                Name = patient.Name,
                BirthDate = patient.BirthDate,
                Genders = patient.Genders.ToString(),
            });
            await _context.SaveChangesAsync();

            return patientId;
        }

        public async Task<Guid> CreateInspection(InspectionCreateModel inspection, Guid patientId, Guid doctorId)
        {
            var previousInspection = await _context.Inspections.Where(x => x.Id == inspection.PreviousInspectionId).FirstOrDefaultAsync();
            if (previousInspection == null && inspection.PreviousInspectionId != null)
            {
                throw new BadHttpRequestException(message: "previous inspection not found");
            }

            var patient = await _context.Patients.Where(x => x.Id == patientId).FirstOrDefaultAsync();
            if (patient == null)
            {
                throw new BadHttpRequestException(message: "patient not found");
            }

            var isd10 = _isd10Context.MedicalRecords.Select(x => x.Id);

            foreach (var diagnosis in inspection.Diagnoses)
            {
                if (!isd10.Contains(diagnosis.IcdDiagnosisId))
                {
                    throw new BadHttpRequestException(message: $"isd10 with id={diagnosis.IcdDiagnosisId} not found");
                }
            }

            if (inspection.Consultations != null)
            {
                var speciality = _context.Specialytis.Select(x => x.Id);

                foreach (var consultation in inspection.Consultations)
                {
                    if (!speciality.Contains(consultation.SpecialityId))
                    {
                        throw new BadHttpRequestException(message: $"speciality with id={consultation.SpecialityId} not found");
                    }
                }
            }

            if (inspection.Date > DateTime.UtcNow)
            {
                throw new BadHttpRequestException(message: $"Date and time can't be later than now {DateTime.UtcNow}");
            }

            if (inspection.NextVisitDate != null && inspection.NextVisitDate < DateTime.UtcNow)
            {
                throw new BadHttpRequestException(message: $"Date and time of the next visit can't be earlier than now {DateTime.UtcNow}");
            }

            var checkDate = await _context.Inspections.Where(x => x.Id == inspection.PreviousInspectionId && x.Date > inspection.Date).FirstOrDefaultAsync();
            if (checkDate != null)
            {
                throw new BadHttpRequestException(message: "Inspection date and time can't be earlier than date and time of previous inspection");
            }

            if (inspection.Conclusion == Conclusion.Death && (inspection.DeathDate == null || inspection.NextVisitDate != null))
            {
                throw new BadHttpRequestException(message: "When choosing the conclusion \"Death\", DeathDate mustn't be null and NextVisitDate must be null");
            }

            if (inspection.Conclusion == Conclusion.Disease && (inspection.NextVisitDate == null || inspection.DeathDate != null))
            {
                throw new BadHttpRequestException(message: "When choosing the conclusion \"Disease\", NextVisitDate mustn't be null and DeathDate must be null");
            }

            if (inspection.Conclusion == Conclusion.Recovery && (inspection.NextVisitDate != null || inspection.DeathDate != null))
            {
                throw new BadHttpRequestException(message: "When choosing the conclusion \"Recovery\", NextVisitDate and DeathDate must be null");
            }

            if (inspection.Diagnoses.Count(x => x.Type == DiagnosisType.Main) != 1)
            {
                throw new BadHttpRequestException(message: "Inspection always must contain one diagnosis with Type equal to Main");
            }

            var checkDeath = await _context.Inspections.Where(x => x.PatientId == patientId && x.Conclusion == Conclusion.Death).FirstOrDefaultAsync();
            if (checkDeath != null)
            {
                throw new BadHttpRequestException(message: "The patient has already died");
            }

            if (inspection.Consultations != null && inspection.Consultations.Select(x => x.SpecialityId).Distinct().Count() != inspection.Consultations.Count)
            {
                throw new BadHttpRequestException(message: "Inspection cannot have several consultations with the same specialty of a doctor");
            }

            if (previousInspection != null)
            {
                if (previousInspection.HasNested)
                {
                    throw new BadHttpRequestException(message: "The inspection already has children inspections");
                }

                if (previousInspection.Conclusion == Conclusion.Recovery)
                {
                    throw new BadHttpRequestException(message: "The patient has already recovered");
                }

                previousInspection.HasChain = previousInspection.PreviousInspectionId == null;
                previousInspection.HasNested = true;
            }

            var inspectionId = Guid.NewGuid();

            await _context.Inspections.AddAsync(new Inspection
            {
                Id = inspectionId,
                CreateTime = DateTime.UtcNow,
                Date = inspection.Date,
                Anamnesis = inspection.Anamnesis,
                Complaints = inspection.Complaints,
                Treatment = inspection.Treatment,
                Conclusion = inspection.Conclusion,
                NextVisitDate = inspection.NextVisitDate,
                DeathDate = inspection.DeathDate,
                BaseInspectionId = inspection.PreviousInspectionId != null ?
                _context.Inspections.Where(x => x.Id == inspection.PreviousInspectionId).Select(x => x.BaseInspectionId).FirstOrDefault() : inspectionId,
                PreviousInspectionId = inspection.PreviousInspectionId,
                PatientId = patientId,
                DoctorId = doctorId,
                HasChain = false,
                HasNested = false
            });

            foreach (var diagnosis in inspection.Diagnoses)
            {
                await _context.Diagnoses.AddAsync(new Diagnosis
                {
                    Id = Guid.NewGuid(),
                    CreateTime = DateTime.UtcNow,
                    IcdDiagnosisId = diagnosis.IcdDiagnosisId,
                    Discription = diagnosis.Discription,
                    Type = diagnosis.Type.ToString(),
                    InspectionId = inspectionId
                });
            }

            if (inspection.Consultations != null)
            {
                foreach (var consultation in inspection.Consultations)
                {
                    var consultationId = Guid.NewGuid();

                    await _context.Consultations.AddAsync(new Consultation
                    {
                        Id = consultationId,
                        CreateTime = DateTime.UtcNow,
                        InspectionId = inspectionId,
                        SpecialityId = consultation.SpecialityId
                    });

                    await _context.Comments.AddAsync(new Comment
                    {
                        Id = Guid.NewGuid(),
                        CreateTime = DateTime.UtcNow,
                        ModifiedDate = null,
                        Content = consultation.comment.Content,
                        Author = doctorId,
                        ParentId = null,
                        CosultationId = consultationId
                    });
                }
            }

            await _context.SaveChangesAsync();

            return inspectionId;
        }

        public async Task<PatientPagedListModel> GetPatient(Guid doctorId, string? name, List<Conclusion> conclusions, PatientSorting? sorting, bool? scheduledVisits,
            bool? onlyMine, int? page, int? size)
        {
            if (size <= 0)
            {
                throw new BadHttpRequestException(message: $"Size value must be greater than 0");
            }

            var patient = await _context.Patients.Include(x => x.Inspections).ToListAsync();

            if (conclusions.Count != 0)
            {
                patient = patient.Where(x => x.Inspections.Any(i => conclusions.Contains(i.Conclusion))).ToList();
            }

            if (onlyMine == true)
            {
                patient = patient.Where(x => x.Inspections.Any(i => i.DoctorId == doctorId)).ToList();
            }

            if (scheduledVisits == true)
            {
                patient = patient.Where(x => x.Inspections.Any(i => !i.HasNested && i.NextVisitDate != null && i.NextVisitDate > DateTime.UtcNow)).ToList();
            }

            patient = SortingDishes(patient, sorting);

            if (name != null)
            {
                patient = patient.Where(x => x.Name.ToLower().Contains(name.ToLower())).ToList();
            }

            var maxPage = (int)((patient.Count() + size - 1) / size);

            if ((page < 1 || patient.Count() <= (page - 1) * size) && maxPage > 0)
            {
                throw new BadHttpRequestException(message: $"Page value must be greater than 0 and less than {maxPage + 1}");
            }

            patient = patient.Skip((int)((page - 1) * size)).Take((int)size).ToList();

            var pagination = new PageInfoModel
            {
                Size = (int)size,
                Count = maxPage,
                Current = (int)page
            };

            return new PatientPagedListModel
            {
                Patients = _mapper.Map<List<PatientModel>>(patient),
                Pagination = pagination
            };
        }

        public async Task<InspectionPagedListModel> GetInspections(Guid patientId, bool? grouped, List<Guid> icdRoots, int? page, int? size)
        {
            if (size <= 0)
            {
                throw new BadHttpRequestException(message: $"Size value must be greater than 0");
            }

            var patient = _context.Patients.Where(x => x.Id == patientId).FirstOrDefault();

            if (patient == null)
            {
                throw new KeyNotFoundException(message: $"Patient not found");
            }

            var isd10 = _isd10Context.MedicalRecords.Select(x => x.Id);

            foreach (var icd in icdRoots)
            {
                if (!isd10.Contains(icd))
                {
                    throw new BadHttpRequestException(message: $"isd10 with id={icd} not found");
                }
            }

            var inspectionWithoutISD = await _context.Inspections.Where(x => x.PatientId == patientId)
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

        public async Task<PatientModel> GetSpecificPatient(Guid patientId)
        {
            var patient = _context.Patients.Where(x => x.Id == patientId).FirstOrDefault();

            if (patient == null)
            {
                throw new KeyNotFoundException(message: "patient not found");
            }

            return _mapper.Map<PatientModel>(patient);
        }

        public async Task<List<InspectionShortModel>> GetInspectionWithoutChild(Guid patientId, string? request)
        {
            var patient = _context.Patients.Where(x => x.Id == patientId).FirstOrDefault();

            if (patient == null)
            {
                throw new KeyNotFoundException(message: $"Patient not found");
            }

            var inspection = await _context.Inspections.Where(x => x.PatientId == patientId && !x.HasNested).Include(x => x.Diagnoses).ToListAsync();

            List<InspectionShortModel> inspections = new List<InspectionShortModel>();

            for (int i = 0; i < inspection.Count; i++)
            {
                var diagnosis = inspection[i].Diagnoses.Where(x => x.Type == DiagnosisType.Main.ToString())
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

                inspections.Add(new InspectionShortModel
                {
                     Id = inspection[i].Id,
                     CreateTime = inspection[i].CreateTime,
                     Date = inspection[i].Date,
                     Diagnosis = new DiagnosisModel
                     {
                         Id = diagnosis.Id,
                         CreateTime = diagnosis.CreateTime,
                         Code = diagnosis.Code,
                         Name = diagnosis.Name,
                         Discription = diagnosis.Discription,
                         Type = diagnosis.Type
                     }
                });
            }

            if (request != null)
            {
                inspections = inspections.Where(x => x.Diagnosis.Name.ToLower().Contains(request.ToLower()) || x.Diagnosis.Code.ToLower().Contains(request.ToLower())).ToList();
            }

            return inspections;
        }

        public List<Patient> SortingDishes(List<Patient> patients, PatientSorting? sorting)
        {
            if (sorting == PatientSorting.NameDesc)
                return patients.OrderByDescending(x => x.Name).ToList();
            if (sorting == PatientSorting.CreateAsc)
                return patients.OrderBy(x => x.CreateTime).ToList();
            if (sorting == PatientSorting.CreateDesc)
                return patients.OrderByDescending(x => x.CreateTime).ToList();
            if (sorting == PatientSorting.InspectionAsc)
                return patients.OrderBy(x => x.Inspections.Count > 0 ? x.Inspections.Min(i => i.Date) : DateTime.MinValue).ToList();
            if (sorting == PatientSorting.InspectionDesc)
                return patients.OrderByDescending(x => x.Inspections.Count > 0 ? x.Inspections.Min(i => i.Date) : DateTime.MinValue).ToList();
            return patients.OrderBy(x => x.Name).ToList();
        }
    }
}
