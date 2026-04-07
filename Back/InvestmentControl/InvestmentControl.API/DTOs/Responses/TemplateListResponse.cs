using InvestmentControl.Application.Analytics.DTOs;

namespace InvestmentControl.API.DTOs.Responses;

public class TemplateListResponse
{
    public List<TemplateDto> Templates { get; set; } = new();
}
