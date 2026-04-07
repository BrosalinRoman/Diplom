using AutoMapper;
using InvestmentControl.Application.Control.DTOs;
using InvestmentControl.Domain.Models;

namespace InvestmentControl.Application.Control.Mappings;

public class ControlProfile : Profile
{
    public ControlProfile()
    {
        CreateMap<Investment, InvestmentDto>();
        CreateMap<Cost, CostDto>();
        CreateMap<ProgressReport, ProgressReportDto>();
    }
}
