﻿using MIS_Backend.DTO;

namespace MIS_Backend.Services.Interfaces
{
    public interface IPatientService
    {
        Task<Guid> CreatePatient(PatientCreateModel patient);
        Task<Guid> CreateInspection(InspectionCreateModel inspection, Guid patientId, Guid doctorId);
    }
}
