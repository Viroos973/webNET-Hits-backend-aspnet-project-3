﻿using MIS_Backend.DTO;

namespace MIS_Backend.Services.Interfaces
{
    public interface IConsultationService
    {
        Task<InspectionPagedListModel> GetInspectionForConsultation(Guid doctorId, bool? grouped, List<Guid> icdRoots, int? page, int? size);
    }
}