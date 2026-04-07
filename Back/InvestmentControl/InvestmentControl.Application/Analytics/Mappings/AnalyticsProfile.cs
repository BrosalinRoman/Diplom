using AutoMapper;
using InvestmentControl.Application.Analytics.DTOs;
using InvestmentControl.Domain.Models;

namespace InvestmentControl.Application.Mappings;

public class AnalyticsProfile : Profile
{
    public AnalyticsProfile()
    {
        CreateMap<Template, TemplateDto>()
            .ForMember(dest => dest.FiltersJson, opt => opt.MapFrom(src => src.FiltersJson))
            .ReverseMap();
        // Можно добавить маппинг ProjectReadModel -> ProjectAnalyticsDto, если нужно
    }
}
