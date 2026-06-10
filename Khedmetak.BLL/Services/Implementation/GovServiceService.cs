using Khedmetak.BLL.DTOS.GovService;
using Khedmetak.BLL.DTOS.GovServiceDetails;
using Khedmetak.BLL.Services.Abstraction;
using Khedmetak.DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khedmetak.BLL.Services.Implementation
{
    public class GovServiceService : IGovServiceService
    {
        private readonly IGovServiceRepository _serviceRepository;

        public GovServiceService(IGovServiceRepository serviceRepository)
        {
            _serviceRepository = serviceRepository;
        }

        public async Task<IEnumerable<GovServiceDto>> GetAllServicesAsync()
        {
            var services = await _serviceRepository.GetAllWithCategoryAsync();
            return services.Select(MapToDto);
        }

        public async Task<IEnumerable<GovServiceDto>> GetServicesByCategoryAsync(int categoryId)
        {
            var services = await _serviceRepository.GetByCategoryAsync(categoryId);
            return services.Select(MapToDto);
        }

        public async Task<GovServiceDetailsDto?> GetServiceDetailsAsync(int id)
        {
            var service = await _serviceRepository.GetServiceWithDetailsAsync(id);
            if (service is null) return null;

            return new GovServiceDetailsDto
            {
                Id = service.Id,
                SrvName = service.SrvName,
                SrvDesc = service.SrvDesc,
                SrvFees = service.SrvFees,
                SrvTime = service.SrvTime,
                EstimatedFees = service.EstimatedFees,
                CategoryId = service.CategoryId,
                CategoryName = service.Category.Name,
                Steps = service.ServiceSteps.Select(s => new ServiceStepDto
                {
                    Id = s.Id,
                    Title = s.Title,
                    StepOrder = s.StepOrder
                }).ToList(),
                RequiredDocuments = service.RequiredDocuments.Select(r => new RequiredDocumentDto
                {
                    Id = r.Id,
                    DocumentName = r.DocumentName,
                    IsMandatory = r.IsMandatory
                }).ToList(),
                Options = service.ServiceOptions.Select(o => new ServiceOptionDto
                {
                    Id = o.Id,
                    Question = o.Question,
                    Choices = o.ServiceOptionChoices.Select(c => new ServiceOptionChoiceDto
                    {
                        Id = c.Id,
                        Choice = c.Choice,
                        IsRequired = c.IsRequired
                    }).ToList()
                }).ToList(),
                GeneralDocs = service.ServiceGeneralDocs.Select(d => new ServiceGeneralDocDto
                {
                    Id = d.Id,
                    Title = d.Title,
                    FilePath = d.FilePath
                }).ToList()
            };
        }

        private static GovServiceDto MapToDto(DAL.Entities.GovService s) => new()
        {
            Id = s.Id,
            SrvName = s.SrvName,
            SrvDesc = s.SrvDesc,
            SrvFees = s.SrvFees,
            SrvTime = s.SrvTime,
            EstimatedFees = s.EstimatedFees,
            CategoryId = s.CategoryId,
            CategoryName = s.Category?.Name ?? string.Empty
        };
    }
}
