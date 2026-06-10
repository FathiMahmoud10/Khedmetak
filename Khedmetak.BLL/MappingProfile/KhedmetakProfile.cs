using AutoMapper;
using Khedmetak.BLL.DTOS.Admin;
using Khedmetak.BLL.DTOS.Categorys;
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
                .ForMember(dest => dest.GeneralDocs,       opt => opt.MapFrom(src => src.ServiceGeneralDocs));

            CreateMap<CreateGovServiceDto, GovService>();
            CreateMap<UpdateGovServiceDto, GovService>();

            CreateMap<ServiceSteps,         ServiceStepDto>();
            CreateMap<RequiredDocument,     RequiredDocumentDto>();
            CreateMap<ServiceGeneralDocs,   ServiceGeneralDocDto>();
            CreateMap<ServiceOptionChoices, ServiceOptionChoiceDto>();
            CreateMap<ServiceOption,        ServiceOptionDto>()
                .ForMember(dest => dest.Choices, opt => opt.MapFrom(src => src.ServiceOptionChoices));
        }
    }
}
