using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MIS_Backend.Database;
using MIS_Backend.DTO;
using MIS_Backend.Services.Interfaces;

namespace MIS_Backend.Services
{
    public class DictionaryService : IDictionaryServices
    {
        private readonly AppDbContext _context;
        private readonly Isd10Context _isd10Context;
        private readonly IMapper _mapper;

        public DictionaryService(AppDbContext context, IMapper mapper, Isd10Context isd10Context)
        {
            _context = context;
            _mapper = mapper;
            _isd10Context = isd10Context;
        }

        public async Task<SpecialtiesPagedListModel> GetSpecialytis(string? name, int? page, int? size)
        {
            if (size <= 0)
            {
                throw new BadHttpRequestException(message: $"Size value must be greater than 0");
            }

            var specialytis = await _context.Specialytis.ToListAsync();

            if (name != null)
            {
                specialytis = specialytis.Where(x => x.Name.ToLower().Contains(name.ToLower())).ToList();
            }

            var maxPage = (int)((specialytis.Count() + size - 1) / size);

            if (page < 1 || specialytis.Count() <= (page - 1) * size)
            {
                throw new BadHttpRequestException(message: $"Page value must be greater than 0 and less than {maxPage + 1}");
            }

            specialytis = specialytis.Skip((int)((page - 1) * size)).Take((int)size).ToList();

            var pagination = new PageInfoModel
            {
                Size = (int)size,
                Count = maxPage,
                Current = (int)page
            };

            return new SpecialtiesPagedListModel
            {
                Specialties = _mapper.Map<List<SpecialityModel>>(specialytis),
                Pagination = pagination
            };
        }

        public async Task<Isd10SearchModel> GetISD10(string? request, int? page, int? size)
        {
            if (size <= 0)
            {
                throw new BadHttpRequestException(message: $"Size value must be greater than 0");
            }

            var isd10 = await _isd10Context.MedicalRecords.ToListAsync();

            if (request != null)
            {
                isd10 = isd10.Where(x => x.MkbName.ToLower().Contains(request.ToLower()) || x.MkbCode.ToLower().Contains(request.ToLower())).ToList();
            }

            var maxPage = (int)((isd10.Count() + size - 1) / size);

            if (page < 1 || isd10.Count() <= (page - 1) * size)
            {
                throw new BadHttpRequestException(message: $"Page value must be greater than 0 and less than {maxPage + 1}");
            }

            isd10 = isd10.Skip((int)((page - 1) * size)).Take((int)size).ToList();

            var pagination = new PageInfoModel
            {
                Size = (int)size,
                Count = maxPage,
                Current = (int)page
            };

            return new Isd10SearchModel
            {
                records = _mapper.Map<List<Isd10RecordModel>>(isd10),
                Pagination = pagination
            };
        }

        public async Task<List<Isd10RecordModel>> GetRootISD10()
        {
            var isd10 = await _isd10Context.MedicalRecords.Where(x => x.IdParent == null).ToListAsync();
            isd10 = isd10.OrderBy(x => x.Id).ToList();
            return _mapper.Map<List<Isd10RecordModel>>(isd10);
        }
    }
}
