    using InvestmentControl.Application.Analytics.DTOs;

namespace InvestmentControl.API.DTOs.Responses;

public class AnalyticsResponse
{
    public List<ProjectAnalyticsDto> Projects { get; set; } = new();
}
