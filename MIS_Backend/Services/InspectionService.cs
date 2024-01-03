using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MIS_Backend.Database;
using MIS_Backend.Database.Enums;
using MIS_Backend.Database.Models;
using MIS_Backend.DTO;
using MIS_Backend.Services.Interfaces;

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
    }
}
