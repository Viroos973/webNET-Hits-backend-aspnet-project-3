using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MIS_Backend.Database;
using MIS_Backend.Database.Enums;
using MIS_Backend.Database.Models;
using MIS_Backend.DTO;
using MIS_Backend.Services.Interfaces;
using System.Runtime.CompilerServices;

namespace MIS_Backend.Services
{
    public class PatientService : IPatientService
    {
        private readonly AppDbContext _context;
        private readonly Isd10Context _isd10Context;

        public PatientService(AppDbContext context, Isd10Context isd10Context)
        {
            _context = context;
            _isd10Context = isd10Context;
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
            if(previousInspection == null && inspection.PreviousInspectionId != null)
            {
                throw new BadHttpRequestException(message: "previous inspection not found");
            }

            if (previousInspection != null && previousInspection.HasNested)
            {
                throw new BadHttpRequestException(message: "The inspection already has children inspections");
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

            if(inspection.Consultations != null)
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

            if (inspection.Conclusion == Conclusion.Death && inspection.DeathDate == null)
            {
                throw new BadHttpRequestException(message: "When choosing the conclusion \"Death\", DeathDate mustn't be null");
            }

            if (inspection.Conclusion == Conclusion.Disease && inspection.NextVisitDate == null)
            {
                throw new BadHttpRequestException(message: "When choosing the conclusion \"Disease\", NextVisitDate mustn't be null");
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

            if(previousInspection != null)
            {
                previousInspection.HasChain = previousInspection.PreviousInspectionId == null;
                previousInspection.HasNested = true;
            }

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
    }
}
