using AutoMapper;
using ClosedXML.Excel;
using Khedmetak.BLL.DTOS.Admin;
using Khedmetak.BLL.DTOS.GovService;
using Khedmetak.BLL.Services.Abstraction;
using Khedmetak.DAL.Entities;
using Khedmetak.DAL.Enums;
using Khedmetak.DAL.Repo.Abstraction.UnitOfWork;
using Khedmetak.DAL.Repositories.Interfaces;
using Shard.VectorDBInterfaces;

namespace Khedmetak.BLL.Services.Implementation
{
    public class GovServiceAdminService : IGovServiceAdminService
    {
        private readonly IGovServiceRepository _serviceRepository;
        private readonly IServiceStepRepository _stepRepository;
        private readonly IRequiredDocumentRepository _docRepository;
        private readonly IServiceFeeTierRepository _feeTierRepository;
        private readonly IServiceImportantNoteRepository _noteRepository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IVectorDBService _vectorDbService;

        public GovServiceAdminService(
            IGovServiceRepository serviceRepository,
            IServiceStepRepository stepRepository,
            IRequiredDocumentRepository docRepository,
            IServiceFeeTierRepository feeTierRepository,
            IServiceImportantNoteRepository noteRepository,
            IMapper mapper, IUnitOfWork unitOfWork,
            IVectorDBService vectorDBService)
        {
            _serviceRepository = serviceRepository;
            _stepRepository = stepRepository;
            _docRepository = docRepository;
            _feeTierRepository = feeTierRepository;
            _noteRepository = noteRepository;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _vectorDbService = vectorDBService;
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

            bool ParseYesNo(string text, bool defaultValue = false)
            {
                if (string.IsNullOrWhiteSpace(text)) return defaultValue;
                var t = text.Trim();
                if (t is "نعم" or "Yes" or "yes" or "True" or "true" or "1" or "إجباري" or "اجباري") return true;
                if (t is "لا" or "No" or "no" or "False" or "false" or "0" or "اختياري") return false;
                return bool.TryParse(t, out var b) ? b : defaultValue;
            }

            // بيسمح بقيم زي "50" أو "50 ج.م" أو "تبدأ من 50 ج.م" أو "50.5" ويستخرج أول رقم منها
            bool TryParseFee(string text, out decimal fee)
            {
                fee = 0;
                if (string.IsNullOrWhiteSpace(text)) return false;
                var normalized = text.Trim()
                    .Replace('٠', '0').Replace('١', '1').Replace('٢', '2').Replace('٣', '3').Replace('٤', '4')
                    .Replace('٥', '5').Replace('٦', '6').Replace('٧', '7').Replace('٨', '8').Replace('٩', '9')
                    .Replace(",", "");
                var match = System.Text.RegularExpressions.Regex.Match(normalized, @"\d+(\.\d+)?");
                return match.Success && decimal.TryParse(match.Value, out fee);
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

            var colProviderEntity = GetColumn("ProviderEntity", "الجهة المقدمة للخدمة", "الجهة المقدمة");
            var colTargetAudience = GetColumn("TargetAudience", "الفئة المستهدفة");
            var colDeliveryMethod = GetColumn("DeliveryMethod", "طريقة الاستلام");
            var colNeedsGuarantee = GetColumn("NeedsGuarantee", "يحتاج ضمان؟", "يحتاج ضمان");

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

                    if (!TryParseFee(feeText, out var fee))
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
                                             TryParseFee(row.Cell(colEstimatedFees).GetString().Trim(), out var estFee)
                                ? estFee
                                : fee,
                            ProviderEntity = colProviderEntity != -1 ? row.Cell(colProviderEntity).GetString().Trim() : string.Empty,
                            TargetAudience = colTargetAudience != -1 ? row.Cell(colTargetAudience).GetString().Trim() : string.Empty,
                            DeliveryMethod = colDeliveryMethod != -1 ? row.Cell(colDeliveryMethod).GetString().Trim() : string.Empty,
                            NeedsGuarantee = colNeedsGuarantee != -1 && ParseYesNo(row.Cell(colNeedsGuarantee).GetString().Trim())
                        };
                        _serviceRepository.Add(service);
                        await _vectorDbService.AddOrUpdateGovServiceToVectorDBAsync(service.Id); // add service to vector database after creation
                       
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
                            TryParseFee(row.Cell(colEstimatedFees).GetString().Trim(), out var estFeeUpdate))
                            service.EstimatedFees = estFeeUpdate;

                        if (colProviderEntity != -1)
                            service.ProviderEntity = row.Cell(colProviderEntity).GetString().Trim();

                        if (colTargetAudience != -1)
                            service.TargetAudience = row.Cell(colTargetAudience).GetString().Trim();

                        if (colDeliveryMethod != -1)
                            service.DeliveryMethod = row.Cell(colDeliveryMethod).GetString().Trim();

                        if (colNeedsGuarantee != -1)
                            service.NeedsGuarantee = ParseYesNo(row.Cell(colNeedsGuarantee).GetString().Trim(), service.NeedsGuarantee);

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
                                var isMandatory = colIsMandatory != -1
                                    ? ParseYesNo(row.Cell(colIsMandatory).GetString().Trim(), true)
                                    : true;

                                var documentType = DocumentType.Any;
                                if (colDocumentType != -1)
                                {
                                    var documentTypeText = row.Cell(colDocumentType).GetString().Trim();
                                    var mapped = documentTypeText switch
                                    {
                                        "صورة" or "صوره" => "Image",
                                        "PDF" or "بي دي إف" or "بي دي اف" => "PDF",
                                        "Word" or "وورد" => "Word",
                                        "أي" or "اي" or "متعدد" or "" => "Any",
                                        _ => documentTypeText
                                    };
                                    if (Enum.TryParse<DocumentType>(mapped, true, out var parsedType))
                                        documentType = parsedType;
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

            // شيت اختياري: "الرسوم والتكاليف" (نفس اسم الشيت في قالب الرفع)
            if (workbook.Worksheets.TryGetWorksheet("الرسوم والتكاليف", out var feeSheet))
            {
                var feeLastRow = feeSheet.LastRowUsed()?.RowNumber() ?? 1;
                if (feeLastRow >= 2)
                {
                    var feeHeaderRow = feeSheet.Row(1);
                    var feeColumnIndex = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
                    var feeLastCol = feeHeaderRow.LastCellUsed()?.Address.ColumnNumber ?? 0;
                    for (int c = 1; c <= feeLastCol; c++)
                    {
                        var headerText = feeHeaderRow.Cell(c).GetString().Trim();
                        if (!string.IsNullOrWhiteSpace(headerText) && !feeColumnIndex.ContainsKey(headerText))
                            feeColumnIndex[headerText] = c;
                    }
                    int GetFeeColumn(params string[] names)
                    {
                        foreach (var n in names)
                            if (feeColumnIndex.TryGetValue(n, out var idx)) return idx;
                        return -1;
                    }

                    var fColServiceName = GetFeeColumn("ServiceName", "اسم الخدمة");
                    var fColTierName = GetFeeColumn("TierName", "نوع الاستمارة");
                    var fColFees = GetFeeColumn("Fees", "الرسوم", "الرسوم (جنيه)");
                    var fColDuration = GetFeeColumn("Duration", "المدة");
                    var fColRefundable = GetFeeColumn("IsRefundable", "قابلة للاسترداد؟", "قابلة للاسترداد");
                    var fColOrder = GetFeeColumn("DisplayOrder", "ترتيب العرض");

                    if (fColServiceName != -1 && fColTierName != -1 && fColFees != -1)
                    {
                        for (int r = 2; r <= feeLastRow; r++)
                        {
                            var row = feeSheet.Row(r);
                            var svcName = row.Cell(fColServiceName).GetString().Trim();
                            var tierName = row.Cell(fColTierName).GetString().Trim();
                            if (string.IsNullOrWhiteSpace(svcName) || string.IsNullOrWhiteSpace(tierName))
                                continue;

                            try
                            {
                                if (!servicesCache.TryGetValue(svcName, out var svc))
                                    throw new InvalidOperationException($"الخدمة '{svcName}' غير موجودة (لم تُعرَّف في الشيت الرئيسي).");

                                if (!TryParseFee(row.Cell(fColFees).GetString().Trim(), out var tierFee))
                                    throw new InvalidOperationException($"قيمة الرسوم غير صحيحة لنوع الاستمارة '{tierName}'.");

                                _feeTierRepository.Add(new ServiceFeeTier
                                {
                                    GovService = svc,
                                    TierName = tierName,
                                    Fees = tierFee,
                                    Duration = fColDuration != -1 ? row.Cell(fColDuration).GetString().Trim() : string.Empty,
                                    IsRefundable = fColRefundable != -1 && ParseYesNo(row.Cell(fColRefundable).GetString().Trim()),
                                    DisplayOrder = fColOrder != -1 && int.TryParse(row.Cell(fColOrder).GetString().Trim(), out var ord) ? ord : r
                                });
                                result.FeeTiersCreated++;
                            }
                            catch (Exception ex)
                            {
                                result.Errors.Add(new ImportRowErrorDto { RowNumber = r, Message = $"[الرسوم والتكاليف] {ex.Message}" });
                            }
                        }
                    }
                }
            }

            // شيت اختياري: "معلومات مهمة" (نفس اسم الشيت في قالب الرفع)
            if (workbook.Worksheets.TryGetWorksheet("معلومات مهمة", out var noteSheet))
            {
                var noteLastRow = noteSheet.LastRowUsed()?.RowNumber() ?? 1;
                if (noteLastRow >= 2)
                {
                    var noteHeaderRow = noteSheet.Row(1);
                    var noteColumnIndex = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
                    var noteLastCol = noteHeaderRow.LastCellUsed()?.Address.ColumnNumber ?? 0;
                    for (int c = 1; c <= noteLastCol; c++)
                    {
                        var headerText = noteHeaderRow.Cell(c).GetString().Trim();
                        if (!string.IsNullOrWhiteSpace(headerText) && !noteColumnIndex.ContainsKey(headerText))
                            noteColumnIndex[headerText] = c;
                    }
                    int GetNoteColumn(params string[] names)
                    {
                        foreach (var n in names)
                            if (noteColumnIndex.TryGetValue(n, out var idx)) return idx;
                        return -1;
                    }

                    var nColServiceName = GetNoteColumn("ServiceName", "اسم الخدمة");
                    var nColNote = GetNoteColumn("Note", "نص الملاحظة");
                    var nColOrder = GetNoteColumn("DisplayOrder", "ترتيب العرض");

                    if (nColServiceName != -1 && nColNote != -1)
                    {
                        for (int r = 2; r <= noteLastRow; r++)
                        {
                            var row = noteSheet.Row(r);
                            var svcName = row.Cell(nColServiceName).GetString().Trim();
                            var noteText = row.Cell(nColNote).GetString().Trim();
                            if (string.IsNullOrWhiteSpace(svcName) || string.IsNullOrWhiteSpace(noteText))
                                continue;

                            try
                            {
                                if (!servicesCache.TryGetValue(svcName, out var svc))
                                    throw new InvalidOperationException($"الخدمة '{svcName}' غير موجودة (لم تُعرَّف في الشيت الرئيسي).");

                                _noteRepository.Add(new ServiceImportantNote
                                {
                                    GovService = svc,
                                    Note = noteText,
                                    DisplayOrder = nColOrder != -1 && int.TryParse(row.Cell(nColOrder).GetString().Trim(), out var ord) ? ord : r
                                });
                                result.ImportantNotesCreated++;
                            }
                            catch (Exception ex)
                            {
                                result.Errors.Add(new ImportRowErrorDto { RowNumber = r, Message = $"[معلومات مهمة] {ex.Message}" });
                            }
                        }
                    }
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
            await _vectorDbService.AddOrUpdateGovServiceToVectorDBAsync(entity.Id); // add service to vector database after creation

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
            entity.ProviderEntity = dto.ProviderEntity;
            entity.TargetAudience = dto.TargetAudience;
            entity.DeliveryMethod = dto.DeliveryMethod;
            entity.NeedsGuarantee = dto.NeedsGuarantee;

            _serviceRepository.Update(entity);
            await _unitOfWork.SaveChangesAsync();
            await _vectorDbService.AddOrUpdateGovServiceToVectorDBAsync(entity.Id); // update service to vector database after Updated


            var updated = await _serviceRepository.GetServiceWithDetailsAsync(id);
            return _mapper.Map<GovServiceDto>(updated);
        }

        public async Task<bool> DeleteServiceAsync(int id)
        {
            var entity = await _serviceRepository.GetByIdAsync(id);
            if (entity is null) return false;

            _serviceRepository.Delete(entity);
            await _unitOfWork.SaveChangesAsync();
            await _vectorDbService.DeleteGovServiceFromVectorDBAsync(entity.Id); // remove service from vector database after deletion

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


        public async Task<IEnumerable<ServiceFeeTierAdminDto>> GetFeeTiersAsync(int govServiceId)
        {
            var tiers = await _feeTierRepository.GetByServiceIdAsync(govServiceId);
            return _mapper.Map<IEnumerable<ServiceFeeTierAdminDto>>(tiers);
        }

        public async Task<ServiceFeeTierAdminDto?> AddFeeTierAsync(int govServiceId, CreateServiceFeeTierDto dto)
        {
            var service = await _serviceRepository.GetByIdAsync(govServiceId);
            if (service is null) return null;

            var entity = _mapper.Map<ServiceFeeTier>(dto);
            entity.GovServiceId = govServiceId;

            _feeTierRepository.Add(entity);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<ServiceFeeTierAdminDto>(entity);
        }

        public async Task<ServiceFeeTierAdminDto?> UpdateFeeTierAsync(int govServiceId, int tierId, UpdateServiceFeeTierDto dto)
        {
            var entity = await _feeTierRepository.GetByIdAsync(tierId);
            if (entity is null || entity.GovServiceId != govServiceId) return null;

            entity.TierName = dto.TierName;
            entity.Fees = dto.Fees;
            entity.Duration = dto.Duration;
            entity.IsRefundable = dto.IsRefundable;
            entity.DisplayOrder = dto.DisplayOrder;

            _feeTierRepository.Update(entity);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<ServiceFeeTierAdminDto>(entity);
        }

        public async Task<bool> DeleteFeeTierAsync(int govServiceId, int tierId)
        {
            var entity = await _feeTierRepository.GetByIdAsync(tierId);
            if (entity is null || entity.GovServiceId != govServiceId) return false;

            _feeTierRepository.Delete(entity);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }


        public async Task<IEnumerable<ServiceImportantNoteAdminDto>> GetImportantNotesAsync(int govServiceId)
        {
            var notes = await _noteRepository.GetByServiceIdAsync(govServiceId);
            return _mapper.Map<IEnumerable<ServiceImportantNoteAdminDto>>(notes);
        }

        public async Task<ServiceImportantNoteAdminDto?> AddImportantNoteAsync(int govServiceId, CreateServiceImportantNoteDto dto)
        {
            var service = await _serviceRepository.GetByIdAsync(govServiceId);
            if (service is null) return null;

            var entity = _mapper.Map<ServiceImportantNote>(dto);
            entity.GovServiceId = govServiceId;

            _noteRepository.Add(entity);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<ServiceImportantNoteAdminDto>(entity);
        }

        public async Task<ServiceImportantNoteAdminDto?> UpdateImportantNoteAsync(int govServiceId, int noteId, UpdateServiceImportantNoteDto dto)
        {
            var entity = await _noteRepository.GetByIdAsync(noteId);
            if (entity is null || entity.GovServiceId != govServiceId) return null;

            entity.Note = dto.Note;
            entity.DisplayOrder = dto.DisplayOrder;

            _noteRepository.Update(entity);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<ServiceImportantNoteAdminDto>(entity);
        }

        public async Task<bool> DeleteImportantNoteAsync(int govServiceId, int noteId)
        {
            var entity = await _noteRepository.GetByIdAsync(noteId);
            if (entity is null || entity.GovServiceId != govServiceId) return false;

            _noteRepository.Delete(entity);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}