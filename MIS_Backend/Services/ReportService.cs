using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MIS_Backend.Database;
using MIS_Backend.Database.Enums;
using MIS_Backend.DTO;
using MIS_Backend.Services.Interfaces;

namespace MIS_Backend.Services
{
    public class ReportService : IReportService
    {
        private readonly AppDbContext _context;
        private readonly Isd10Context _isd10Context;

        public ReportService(AppDbContext context, Isd10Context isd10Context)
        {
            _context = context;
            _isd10Context = isd10Context;
        }

        public async Task<IcdRootsReportModel> GetReport(DateTime start, DateTime end, List<Guid> icdRoots)
        {
            var isd10 = await _isd10Context.MedicalRecords.Where(x => x.IdParent == null).Select(x => x.Id).ToListAsync();

            foreach (var icd in icdRoots)
            {
                if (!isd10.Contains(icd))
                {
                    throw new BadHttpRequestException(message: $"isd10 root with id={icd} not found");
                }
            }

            if (start > end)
            {
                throw new BadHttpRequestException(message: "Invalid time interval");
            }

            Dictionary<string, int> countIcdRoot = new Dictionary<string, int>();
            List<IcdRootsReportRecordModel> records = new List<IcdRootsReportRecordModel>();

            if ( icdRoots.Count == 0)
            {
                icdRoots = isd10;
            }

            var icdRootsCode = await _isd10Context.MedicalRecords.Where(x => icdRoots.Contains(x.Id)).Select(x => new { x.MkbCode, x.Id }).ToListAsync();

            foreach (var icd in icdRootsCode)
            {
                countIcdRoot[icd.MkbCode] = 0;
            }

            var patients = await _context.Patients
                .Include(x => x.Inspections).ThenInclude(x => x.Diagnoses)
                .Where(x => x.Inspections.Any(i => i.Date >= start && i.Date <= end)).ToListAsync();

            foreach (var patient in patients)
            {
                Dictionary<string, int> countIcdRootPatient = new Dictionary<string, int>();

                var inspections = patient.Inspections.Where(i => i.Date >= start && i.Date <= end).ToList();
                
                foreach (var inspection in inspections)
                {
                    var root = inspection.Diagnoses.Where(x => x.Type == DiagnosisType.Main.ToString()).Join(_isd10Context.MedicalRecords,
                      d => d.IcdDiagnosisId,
                      m => m.Id,
                      (d, m) => m.Root).First();

                    var code = icdRootsCode.Where(x => x.Id == root).FirstOrDefault();

                    if (code != null)
                    {
                        if (countIcdRootPatient.ContainsKey(code.MkbCode))
                        {
                            countIcdRootPatient[code.MkbCode]++;
                        }
                        else
                        {
                            countIcdRootPatient[code.MkbCode] = 1;
                        }

                        countIcdRoot[code.MkbCode]++;
                    }
                }

                if (countIcdRootPatient.Count > 0)
                records.Add(new IcdRootsReportRecordModel
                {
                    PatientName = patient.Name,
                    PatientBirthDate = patient.BirthDate,
                    Gender = patient.Genders,
                    VisitByRoots = countIcdRootPatient.OrderBy(x => x.Key).ToDictionary(x => x.Key, x => x.Value),
                });
            }

            return new IcdRootsReportModel
            {
                Filters = new IcdRootsReportFiltersModel
                {
                    Start = start,
                    End = end,
                    IsdRoots = icdRootsCode.Select(x => x.MkbCode).OrderBy(x => x).ToList()
        },
                Records = records.OrderBy(x => x.PatientName).ToList(),
                SummaryByRoot = countIcdRoot.OrderBy(x => x.Key).ToDictionary(x => x.Key, x => x.Value)
            };
        }
    }
}
