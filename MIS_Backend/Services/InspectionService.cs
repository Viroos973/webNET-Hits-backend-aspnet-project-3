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
    public class InspectionService : IInspectionService
    {
        private readonly AppDbContext _context;
        private readonly Isd10Context _isd10Context;
        private readonly IMapper _mapper;

        public InspectionService(AppDbContext context, Isd10Context isd10Context, IMapper mapper) 
        {
            _context = context;
            _isd10Context = isd10Context;
            _mapper = mapper;
        }

        public async Task<InspectionModel> GetSpecificInspection(Guid inspectionId)
        {
            var inspection = _context.Inspections.Where(x => x.Id == inspectionId)
                .Include(x => x.Diagnoses)
                .Include(x => x.Patients)
                .Include(x => x.Doctors)
                .Include(x => x.Consultations)
                .ThenInclude(c => c.Specialytis)
                .Include(x => x.Consultations)
                .ThenInclude(x => x.Comments)
                .ThenInclude(s => s.Doctors)
                .FirstOrDefault();

            if (inspection == null)
            {
                throw new KeyNotFoundException(message: $"Inspection not found");
            }

            var diagnosis = inspection.Diagnoses.Join(_isd10Context.MedicalRecords,
                      d => d.IcdDiagnosisId,
                      m => m.Id,
                      (d, m) => new DiagnosisModel
                      {
                          Id = d.Id,
                          CreateTime = d.CreateTime,
                          Code = m.MkbCode,
                          Name = m.MkbName,
                          Discription = d.Discription,
                          Type = d.Type
                      }).ToList();

            return new InspectionModel
            {
                Id = inspection.Id,
                Anamnesis = inspection.Anamnesis,
                Complaints = inspection.Complaints,
                Treatment = inspection.Treatment,
                CreateTime = inspection.CreateTime,
                PreviousInspectionId = inspection.PreviousInspectionId,
                Date = inspection.Date,
                Conclusion = inspection.Conclusion,
                NextVisitDate = inspection.NextVisitDate,
                DeathDate = inspection.DeathDate,
                BaseInspectionId = inspection.BaseInspectionId,
                Patient = _mapper.Map<PatientModel>(inspection.Patients),
                Doctor = _mapper.Map<DoctorModel>(inspection.Doctors),
                Diagnoses = diagnosis,
                Consultations = _mapper.Map<List<InspectionConsultationModel>>(inspection.Consultations)
            };
        }

        public async Task EdiInspection(InspectionEditModel inspectionEdit, Guid inspectionId, Guid doctorId)
        {
            var inspection = _context.Inspections.Where(x => x.Id == inspectionId).FirstOrDefault();

            if (inspection == null)
            {
                throw new KeyNotFoundException(message: "inspection not found");
            }

            if (inspection.DoctorId != doctorId)
            {
                throw new SecurityException(message: $"The user is not the author of the inspection with id={inspectionId}");
            }

            var isd10 = _isd10Context.MedicalRecords.Select(x => x.Id);

            foreach (var diagnoses in inspectionEdit.Diagnoses)
            {
                if (!isd10.Contains(diagnoses.IcdDiagnosisId))
                {
                    throw new BadHttpRequestException(message: $"isd10 with id={diagnoses.IcdDiagnosisId} not found");
                }
            }

            if (inspectionEdit.Conclusion == Conclusion.Death && (inspectionEdit.DeathDate == null || inspectionEdit.NextVisitDate != null))
            {
                throw new BadHttpRequestException(message: "When choosing the conclusion 'Death', DeathDate mustn't be null and NextVisitDate must be null");
            }

            if (inspectionEdit.Conclusion == Conclusion.Disease && (inspectionEdit.NextVisitDate == null || inspectionEdit.DeathDate != null))
            {
                throw new BadHttpRequestException(message: "When choosing the conclusion 'Disease', NextVisitDate mustn't be null and DeathDate must be null");
            }

            if (inspectionEdit.Conclusion == Conclusion.Recovery && (inspectionEdit.NextVisitDate != null || inspectionEdit.DeathDate != null))
            {
                throw new BadHttpRequestException(message: "When choosing the conclusion 'Recovery', NextVisitDate and DeathDate must be null");
            }

            if (inspectionEdit.Conclusion != Conclusion.Disease && inspection.HasNested)
            {
                throw new BadHttpRequestException(message: "The conclusion must be 'Disease', because the patient has later examinations");
            }

            if (inspectionEdit.NextVisitDate != null && inspectionEdit.NextVisitDate < inspection.Date)
            {
                throw new BadHttpRequestException(message: "Date and time of the next visit can't be earlier than date");
            }

            if (inspectionEdit.DeathDate != null && inspectionEdit.DeathDate < inspection.Date)
            {
                throw new BadHttpRequestException(message: "Date and time of the death can't be earlier than date");
            }

            if (inspectionEdit.Diagnoses.Count(x => x.Type == DiagnosisType.Main) != 1)
            {
                throw new BadHttpRequestException(message: "Inspection always must contain one diagnosis with Type equal to Main");
            }

            inspection.Anamnesis = inspectionEdit.Anamnesis;
            inspection.Complaints = inspectionEdit.Complaints;
            inspection.Treatment = inspectionEdit.Treatment;
            inspection.Conclusion = inspectionEdit.Conclusion;
            inspection.NextVisitDate = inspectionEdit.NextVisitDate;
            inspection.DeathDate = inspectionEdit.DeathDate;

            var diagnosis = await _context.Diagnoses.Where(x => x.InspectionId == inspectionId).ToListAsync();
            _context.Diagnoses.RemoveRange(diagnosis);

            foreach (var diagnosisEdit in inspectionEdit.Diagnoses)
            {
                await _context.Diagnoses.AddAsync(new Diagnosis
                {
                    Id = Guid.NewGuid(),
                    CreateTime = DateTime.UtcNow,
                    IcdDiagnosisId = diagnosisEdit.IcdDiagnosisId,
                    Discription = diagnosisEdit.Discription,
                    Type = diagnosisEdit.Type.ToString(),
                    InspectionId = inspectionId
                });
            }

            await _context.SaveChangesAsync();
        }

        public async Task<List<InspectionPreviewModel>> GetInspectionChain(Guid inspectionId)
        {
            var inspectionRoot = _context.Inspections.Where(x => x.Id == inspectionId).FirstOrDefault();

            if (inspectionRoot == null)
            {
                throw new KeyNotFoundException(message: $"Inspection not found");
            }

            if (inspectionRoot.PreviousInspectionId != null)
            {
                throw new BadHttpRequestException(message: $"Try to get chain for non-root medical inspection with id={inspectionId}");
            }

            var inspectionChild = await _context.Inspections.Where(x => x.BaseInspectionId == inspectionId)
                .Include(x => x.Diagnoses)
                .Include(x => x.Patients)
                .Include(x => x.Doctors).ToListAsync();

            List<InspectionPreviewModel> inspections = new List<InspectionPreviewModel>();

            for (int i = 0; i < inspectionChild.Count; i++)
            {
                var diagnosis = inspectionChild[i].Diagnoses.Where(x => x.Type == DiagnosisType.Main.ToString())
                    .Join(_isd10Context.MedicalRecords,
                      d => d.IcdDiagnosisId,
                      m => m.Id,
                      (d, m) => new DiagnosisModel
                      {
                          Id = d.Id,
                          CreateTime = d.CreateTime,
                          Code = m.MkbCode,
                          Name = m.MkbName,
                          Discription = d.Discription,
                          Type = d.Type
                      }).First();

                inspections.Add(new InspectionPreviewModel
                {
                    Id = inspectionChild[i].Id,
                    CreateTime = inspectionChild[i].CreateTime,
                    PreviousId = inspectionChild[i].PreviousInspectionId,
                    Date = inspectionChild[i].Date,
                    Conclusion = inspectionChild[i].Conclusion,
                    PatientId = inspectionChild[i].PatientId,
                    Patient = inspectionChild[i].Patients.Name,
                    DoctorId = inspectionChild[i].DoctorId,
                    Doctor = inspectionChild[i].Doctors.Name,
                    Diagnosis = diagnosis,
                    HasChain = inspectionChild[i].HasChain,
                    HasNested = inspectionChild[i].HasNested
                });
            }

            return inspections.OrderBy(x => x.CreateTime).ToList();
        }
    }
}
