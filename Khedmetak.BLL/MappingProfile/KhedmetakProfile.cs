using AutoMapper;
using Khedmetak.BLL.DTOS.Admin;
using Khedmetak.BLL.DTOS.Categorys;
using Khedmetak.BLL.DTOS.Documents;
using Khedmetak.BLL.DTOS.GovService;
using Khedmetak.BLL.DTOS.GovServiceDetails;
using Khedmetak.DAL.Entities;

namespace Khedmetak.BLL.MappingProfile
{
    public class KhedmetakProfile : Profile
    {
        public KhedmetakProfile()
        {
            CreateMap<Category, CategoryDto>()
                .ForMember(dest => dest.ServicesCount, opt => opt.MapFrom(src => src.GovServices.Count));

            CreateMap<CreateCategoryDto, Category>();
            CreateMap<UpdateCategoryDto, Category>();

            CreateMap<GovService, GovServiceDto>()
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name));

            CreateMap<GovService, GovServiceDetailsDto>()
                .ForMember(dest => dest.CategoryName,      opt => opt.MapFrom(src => src.Category.Name))
                .ForMember(dest => dest.Steps,             opt => opt.MapFrom(src => src.ServiceSteps))
                .ForMember(dest => dest.RequiredDocuments, opt => opt.MapFrom(src => src.RequiredDocuments))
                .ForMember(dest => dest.Options,           opt => opt.MapFrom(src => src.ServiceOptions))
                .ForMember(dest => dest.GeneralDocs,       opt => opt.MapFrom(src => src.ServiceGeneralDocs))
                .ForMember(dest => dest.FeeTiers,          opt => opt.MapFrom(src => src.ServiceFeeTiers))
                .ForMember(dest => dest.ImportantNotes,    opt => opt.MapFrom(src => src.ImportantNotes));

            CreateMap<CreateGovServiceDto, GovService>();
            CreateMap<UpdateGovServiceDto, GovService>();
            CreateMap<UserDocument, UserDocumentDto>();

            CreateMap<ServiceSteps,         ServiceStepDto>();
            CreateMap<RequiredDocument,     RequiredDocumentDto>();
            CreateMap<ServiceGeneralDocs,   ServiceGeneralDocDto>();
            CreateMap<ServiceOptionChoices, ServiceOptionChoiceDto>();
            CreateMap<ServiceOption,        ServiceOptionDto>()
                .ForMember(dest => dest.Choices, opt => opt.MapFrom(src => src.ServiceOptionChoices));



            CreateMap<CreateGovServiceDto, GovService>();
            CreateMap<UpdateGovServiceDto, GovService>();
            CreateMap<CreateServiceStepDto, ServiceSteps>();
            CreateMap<UpdateServiceStepDto, ServiceSteps>();
            CreateMap<CreateRequiredDocumentDto, RequiredDocument>();
            CreateMap<UpdateRequiredDocumentDto, RequiredDocument>();

            CreateMap<ServiceSteps, ServiceStepAdminDto>();
            CreateMap<RequiredDocument, RequiredDocumentAdminDto>();

            CreateMap<ServiceFeeTier, ServiceFeeTierDto>();
            CreateMap<ServiceFeeTier, ServiceFeeTierAdminDto>();
            CreateMap<CreateServiceFeeTierDto, ServiceFeeTier>();
            CreateMap<UpdateServiceFeeTierDto, ServiceFeeTier>();

            CreateMap<ServiceImportantNote, ServiceImportantNoteDto>();
            CreateMap<ServiceImportantNote, ServiceImportantNoteAdminDto>();
            CreateMap<CreateServiceImportantNoteDto, ServiceImportantNote>();
            CreateMap<UpdateServiceImportantNoteDto, ServiceImportantNote>();
        }
    }
}
