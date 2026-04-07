using InvestmentControl.Application.Analytics.DTOs;

namespace InvestmentControl.API.DTOs.Responses;

public class AnalyticsResponse
{
    public List<ProjectAnalyticsDto> Projects { get; set; } = new();
    // Здесь можно добавить информацию о характеристиках и диапазонах
}
