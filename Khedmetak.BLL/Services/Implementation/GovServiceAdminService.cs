using AutoMapper;
using ClosedXML.Excel;
using Khedmetak.BLL.DTOS.Admin;
using Khedmetak.BLL.DTOS.GovService;
using Khedmetak.BLL.Services.Abstraction;
using Khedmetak.DAL.Entities;
using Khedmetak.DAL.Enums;
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
            IMapper mapper, IUnitOfWork unitOfWork)
        {
            _serviceRepository = serviceRepository;
            _stepRepository = stepRepository;
            _docRepository = docRepository;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }


        public async Task<ImportServicesResultDto> ImportServicesFromExcelAsync(Stream excelFileStream)
        {
            var result = new ImportServicesResultDto();

            using var workbook = new XLWorkbook(excelFileStream);
            var sheet = workbook.Worksheets.First();

            var lastRow = sheet.LastRowUsed()?.RowNumber() ?? 1;
            if (lastRow < 2)
                return result; 

            var headerRow = sheet.Row(1);
            var columnIndex = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            var lastCol = headerRow.LastCellUsed()?.Address.ColumnNumber ?? 0;
            for (int c = 1; c <= lastCol; c++)
            {
                var headerText = headerRow.Cell(c).GetString().Trim();
                if (!string.IsNullOrWhiteSpace(headerText) && !columnIndex.ContainsKey(headerText))
                    columnIndex[headerText] = c;
            }

            int GetColumn(params string[] possibleNames)
            {
                foreach (var name in possibleNames)
                    if (columnIndex.TryGetValue(name, out var idx))
                        return idx;
                return -1;
            }

            var colServiceName = GetColumn("ServiceName", "اسم الخدمة");
            var colCategory = GetColumn("Category", "التصنيف", "الفئة");
            var colFee = GetColumn("Fee", "الرسوم");
            var colStepOrder = GetColumn("StepOrder", "ترتيب الخطوة");
            var colStepTitle = GetColumn("StepTitle", "عنوان الخطوة");
            var colDocumentName = GetColumn("DocumentName", "اسم المستند");

            var colSrvDesc = GetColumn("SrvDesc", "وصف الخدمة");
            var colSrvTime = GetColumn("SrvTime", "مدة الخدمة");
            var colEstimatedFees = GetColumn("EstimatedFees", "الرسوم التقديرية");
            var colIsMandatory = GetColumn("IsMandatory", "مستند إجباري");
            var colDocumentType = GetColumn("DocumentType", "نوع المستند");

            if (colServiceName == -1 || colCategory == -1 || colFee == -1)
            {
                result.Errors.Add(new ImportRowErrorDto
                {
                    RowNumber = 1,
                    Message = "الأعمدة الأساسية مفقودة من الـ Header: لازم يكون فيه ServiceName و Category و Fee."
                });
                return result;
            }

            var existingCategories = await _unitOfWork.Categories.GetAllAsync();
            var categoriesCache = existingCategories
                .GroupBy(c => c.Name.Trim(), StringComparer.OrdinalIgnoreCase)
                .ToDictionary(g => g.Key, g => g.First(), StringComparer.OrdinalIgnoreCase);

            var existingServices = await _serviceRepository.GetAllAsync();
            var servicesCache = existingServices
                .GroupBy(s => s.SrvName.Trim(), StringComparer.OrdinalIgnoreCase)
                .ToDictionary(g => g.Key, g => g.First(), StringComparer.OrdinalIgnoreCase);

            var stepKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var existingSteps = await _stepRepository.GetAllAsync(s => s.GovService);
            foreach (var step in existingSteps)
                stepKeys.Add($"{step.GovService.SrvName.Trim()}|{step.StepOrder}|{step.Title.Trim()}");

            var docKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var existingDocs = await _docRepository.GetAllAsync(d => d.GovService);
            foreach (var doc in existingDocs)
                docKeys.Add($"{doc.GovService.SrvName.Trim()}|{doc.DocumentName.Trim()}");

            for (int rowNum = 2; rowNum <= lastRow; rowNum++)
            {
                var row = sheet.Row(rowNum);
                result.TotalRowsRead++;

                var serviceName = row.Cell(colServiceName).GetString().Trim();
                if (string.IsNullOrWhiteSpace(serviceName))
                    continue; 

                try
                {
                    var categoryName = row.Cell(colCategory).GetString().Trim();
                    var feeText = row.Cell(colFee).GetString().Trim();

                    if (string.IsNullOrWhiteSpace(categoryName))
                        throw new InvalidOperationException("اسم التصنيف (Category) فاضي.");

                    if (!decimal.TryParse(feeText, out var fee))
                        throw new InvalidOperationException($"قيمة الرسوم (Fee) غير صحيحة: '{feeText}'.");

                    if (!categoriesCache.TryGetValue(categoryName, out var category))
                    {
                        category = new Category { Name = categoryName };
                        _unitOfWork.Categories.Add(category);
                        categoriesCache[categoryName] = category;
                        result.CategoriesCreated++;
                    }

                    if (!servicesCache.TryGetValue(serviceName, out var service))
                    {
                        service = new GovService
                        {
                            SrvName = serviceName,
                            SrvFees = fee,
                            Category = category,
                            SrvDesc = colSrvDesc != -1 ? row.Cell(colSrvDesc).GetString().Trim() : string.Empty,
                            SrvTime = colSrvTime != -1 ? row.Cell(colSrvTime).GetString().Trim() : string.Empty,
                            EstimatedFees = colEstimatedFees != -1 &&
                                             decimal.TryParse(row.Cell(colEstimatedFees).GetString().Trim(), out var estFee)
                                ? estFee
                                : fee
                        };
                        _serviceRepository.Add(service);
                        servicesCache[serviceName] = service;
                        result.ServicesCreated++;
                    }
                    else
                    {
                       service.SrvFees = fee;
                        service.Category = category;

                        if (colSrvDesc != -1)
                            service.SrvDesc = row.Cell(colSrvDesc).GetString().Trim();

                        if (colSrvTime != -1)
                            service.SrvTime = row.Cell(colSrvTime).GetString().Trim();

                        if (colEstimatedFees != -1 &&
                            decimal.TryParse(row.Cell(colEstimatedFees).GetString().Trim(), out var estFeeUpdate))
                            service.EstimatedFees = estFeeUpdate;

                        _serviceRepository.Update(service);
                        result.ServicesUpdated++;
                    }

                    if (colStepOrder != -1 && colStepTitle != -1)
                    {
                        var stepTitle = row.Cell(colStepTitle).GetString().Trim();
                        var stepOrderText = row.Cell(colStepOrder).GetString().Trim();

                        if (!string.IsNullOrWhiteSpace(stepTitle))
                        {
                            if (!int.TryParse(stepOrderText, out var stepOrder))
                                throw new InvalidOperationException($"ترتيب الخطوة (StepOrder) غير صحيح: '{stepOrderText}'.");

                            var stepKey = $"{serviceName}|{stepOrder}|{stepTitle}";
                            if (stepKeys.Add(stepKey))
                            {
                                _stepRepository.Add(new ServiceSteps
                                {
                                    GovService = service,
                                    StepOrder = stepOrder,
                                    Title = stepTitle
                                });
                                result.StepsCreated++;
                            }
                        }
                    }

                    if (colDocumentName != -1)
                    {
                        var documentName = row.Cell(colDocumentName).GetString().Trim();
                        if (!string.IsNullOrWhiteSpace(documentName))
                        {
                            var docKey = $"{serviceName}|{documentName}";
                            if (docKeys.Add(docKey))
                            {
                                var isMandatory = true;
                                if (colIsMandatory != -1)
                                {
                                    var isMandatoryText = row.Cell(colIsMandatory).GetString().Trim();
                                    if (!string.IsNullOrWhiteSpace(isMandatoryText))
                                        bool.TryParse(isMandatoryText, out isMandatory);
                                }

                                var documentType = DocumentType.Any;
                                if (colDocumentType != -1)
                                {
                                    var documentTypeText = row.Cell(colDocumentType).GetString().Trim();
                                    if (!string.IsNullOrWhiteSpace(documentTypeText))
                                        Enum.TryParse(documentTypeText, true, out documentType);
                                }

                                _docRepository.Add(new RequiredDocument
                                {
                                    GovService = service,
                                    DocumentName = documentName,
                                    IsMandatory = isMandatory,
                                    DocumentType = documentType
                                });
                                result.DocumentsCreated++;
                            }
                        }
                    }

                    result.RowsProcessed++;
                }
                catch (Exception ex)
                {
                    result.Errors.Add(new ImportRowErrorDto
                    {
                        RowNumber = rowNum,
                        Message = ex.Message
                    });
                }
            }

            await _unitOfWork.SaveChangesAsync();

            return result;
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