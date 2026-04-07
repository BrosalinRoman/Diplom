using InvestmentControl.Application.Control.DTOs;

namespace InvestmentControl.API.DTOs.Responses;

public class ControlProjectsResponse
{
    public List<ControlProjectDto> Projects { get; set; } = new();
    public int TotalCount { get; set; } // если нужна пагинация
}
