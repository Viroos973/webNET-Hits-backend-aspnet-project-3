using MIS_Backend.Database;
using MIS_Backend.Database.Models;
using MIS_Backend.DTO;
using MIS_Backend.Services.Interfaces;

namespace MIS_Backend.Services
{
    public class PatientService : IPatientService
    {
        private readonly AppDbContext _context;

        public PatientService(AppDbContext context)
        {
            _context = context;
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
    }
}
