using AutoMapper;
using Khedmetak.BLL.DTOS.Admin;
using Khedmetak.BLL.DTOS.GovService;
using Khedmetak.BLL.DTOS.GovServiceDetails;
using Khedmetak.BLL.Services.Abstraction;
using Khedmetak.DAL.Entities;
using Khedmetak.DAL.Repo.Abstraction.UnitOfWork;

namespace Khedmetak.BLL.Services.Implementation
{
    public class GovServiceService : IGovServiceService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GovServiceService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }


        public async Task<IEnumerable<GovServiceDto>> GetAllServicesAsync()
        {
            var services = await _unitOfWork.GovServices.GetAllWithCategoryAsync();
            return _mapper.Map<IEnumerable<GovServiceDto>>(services);
        }

        public async Task<IEnumerable<GovServiceDto>> GetServicesByCategoryAsync(int categoryId)
        {
            var services = await _unitOfWork.GovServices.GetByCategoryAsync(categoryId);
            return _mapper.Map<IEnumerable<GovServiceDto>>(services);
        }

        public async Task<GovServiceDetailsDto?> GetServiceDetailsAsync(int id)
        {
            var service = await _unitOfWork.GovServices.GetServiceWithDetailsAsync(id);
            if (service is null) return null;
            return _mapper.Map<GovServiceDetailsDto>(service);
        }


        public async Task<GovServiceDto> CreateServiceAsync(CreateGovServiceDto dto)
        {
            var entity = _mapper.Map<GovService>(dto);
            _unitOfWork.GovServices.Add(entity);
            await _unitOfWork.SaveChangesAsync();

            var created = await _unitOfWork.GovServices.GetServiceWithDetailsAsync(entity.Id);
            return _mapper.Map<GovServiceDto>(created);
        }

        public async Task<GovServiceDto?> UpdateServiceAsync(int id, UpdateGovServiceDto dto)
        {
            var entity = await _unitOfWork.GovServices.GetByIdAsync(id);
            if (entity is null) return null;

            _mapper.Map(dto, entity);
            _unitOfWork.GovServices.Update(entity);
            await _unitOfWork.SaveChangesAsync();

            var updated = await _unitOfWork.GovServices.GetServiceWithDetailsAsync(entity.Id);
            return _mapper.Map<GovServiceDto>(updated);
        }

        public async Task<bool> DeleteServiceAsync(int id)
        {
            var entity = await _unitOfWork.GovServices.GetByIdAsync(id);
            if (entity is null) return false;

            _unitOfWork.GovServices.Delete(entity);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
