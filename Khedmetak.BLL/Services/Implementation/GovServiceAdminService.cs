using AutoMapper;
using Khedmetak.BLL.DTOS.Admin;
using Khedmetak.BLL.DTOS.GovService;
using Khedmetak.BLL.Services.Abstraction;
using Khedmetak.DAL.Entities;
using Khedmetak.DAL.Repo.Abstraction.UnitOfWork;
using Khedmetak.DAL.Repositories.Interfaces;

namespace Khedmetak.BLL.Services.Implementation
{
    public class GovServiceAdminService : IGovServiceAdminService
    {
        private readonly IGovServiceRepository _serviceRepository;
        private readonly IServiceStepRepository _stepRepository;
        private readonly IRequiredDocumentRepository _docRepository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        public GovServiceAdminService(
            IGovServiceRepository serviceRepository,
            IServiceStepRepository stepRepository,
            IRequiredDocumentRepository docRepository,
            IMapper mapper , IUnitOfWork unitOfWork)
        {
            _serviceRepository = serviceRepository;
            _stepRepository = stepRepository;
            _docRepository = docRepository;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        
        public async Task<GovServiceDto> CreateServiceAsync(CreateGovServiceDto dto)
        {
            var entity = _mapper.Map<GovService>(dto);

            _serviceRepository.Add(entity);
            await _unitOfWork.SaveChangesAsync();

            var created = await _serviceRepository.GetServiceWithDetailsAsync(entity.Id);
            return _mapper.Map<GovServiceDto>(created);
        }

        public async Task<GovServiceDto?> UpdateServiceAsync(int id, UpdateGovServiceDto dto)
        {
            var entity = await _serviceRepository.GetByIdAsync(id);
            if (entity is null) return null;

            entity.SrvName = dto.SrvName;
            entity.SrvDesc = dto.SrvDesc;
            entity.SrvTime = dto.SrvTime;
            entity.CategoryId = dto.CategoryId;

            _serviceRepository.Update(entity);
            await _unitOfWork.SaveChangesAsync();

            var updated = await _serviceRepository.GetServiceWithDetailsAsync(id);
            return _mapper.Map<GovServiceDto>(updated);
        }

        public async Task<bool> DeleteServiceAsync(int id)
        {
            var entity = await _serviceRepository.GetByIdAsync(id);
            if (entity is null) return false;

            _serviceRepository.Delete(entity);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        
        public async Task<GovServiceDto?> UpdateFeesAsync(int id, UpdateFeesDto dto)
        {
            var entity = await _serviceRepository.GetByIdAsync(id);
            if (entity is null) return null;

            entity.SrvFees = dto.SrvFees;
            entity.EstimatedFees = dto.EstimatedFees;

            _serviceRepository.Update(entity);
            await _unitOfWork.SaveChangesAsync();

            var updated = await _serviceRepository.GetServiceWithDetailsAsync(id);
            return _mapper.Map<GovServiceDto>(updated);
        }

     

        public async Task<IEnumerable<ServiceStepAdminDto>> GetStepsAsync(int govServiceId)
        {
            var steps = await _stepRepository.GetByServiceIdAsync(govServiceId);
            return _mapper.Map<IEnumerable<ServiceStepAdminDto>>(steps);
        }

        public async Task<ServiceStepAdminDto?> AddStepAsync(int govServiceId, CreateServiceStepDto dto)
        {
            var service = await _serviceRepository.GetByIdAsync(govServiceId);
            if (service is null) return null;

            var entity = _mapper.Map<ServiceSteps>(dto);
            entity.GovServiceId = govServiceId;

            _stepRepository.Add(entity);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<ServiceStepAdminDto>(entity);
        }

        public async Task<ServiceStepAdminDto?> UpdateStepAsync(int govServiceId, int stepId, UpdateServiceStepDto dto)
        {
            var entity = await _stepRepository.GetByIdAsync(stepId);
            if (entity is null || entity.GovServiceId != govServiceId) return null;

            entity.Title = dto.Title;
            entity.StepOrder = dto.StepOrder;

            _stepRepository.Update(entity);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<ServiceStepAdminDto>(entity);
        }

        public async Task<bool> DeleteStepAsync(int govServiceId, int stepId)
        {
            var entity = await _stepRepository.GetByIdAsync(stepId);
            if (entity is null || entity.GovServiceId != govServiceId) return false;

            _stepRepository.Delete(entity);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }


        public async Task<IEnumerable<RequiredDocumentAdminDto>> GetRequiredDocumentsAsync(int govServiceId)
        {
            var docs = await _docRepository.GetByServiceIdAsync(govServiceId);
            return _mapper.Map<IEnumerable<RequiredDocumentAdminDto>>(docs);
        }

        public async Task<RequiredDocumentAdminDto?> AddRequiredDocumentAsync(int govServiceId, CreateRequiredDocumentDto dto)
        {
            var service = await _serviceRepository.GetByIdAsync(govServiceId);
            if (service is null) return null;

            var entity = _mapper.Map<RequiredDocument>(dto);
            entity.GovServiceId = govServiceId;

            _docRepository.Add(entity);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<RequiredDocumentAdminDto>(entity);
        }

        public async Task<RequiredDocumentAdminDto?> UpdateRequiredDocumentAsync(int govServiceId, int docId, UpdateRequiredDocumentDto dto)
        {
            var entity = await _docRepository.GetByIdAsync(docId);
            if (entity is null || entity.GovServiceId != govServiceId) return null;

            entity.DocumentName = dto.DocumentName;
            entity.IsMandatory = dto.IsMandatory;
            entity.DocumentType = dto.DocumentType;

            _docRepository.Update(entity);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<RequiredDocumentAdminDto>(entity);
        }

        public async Task<bool> DeleteRequiredDocumentAsync(int govServiceId, int docId)
        {
            var entity = await _docRepository.GetByIdAsync(docId);
            if (entity is null || entity.GovServiceId != govServiceId) return false;

            _docRepository.Delete(entity);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
