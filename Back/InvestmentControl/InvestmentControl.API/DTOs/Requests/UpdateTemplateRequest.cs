namespace InvestmentControl.API.DTOs.Requests;

public class UpdateTemplateRequest
{
    public string Name { get; set; } = string.Empty;
    public string FiltersJson { get; set; } = string.Empty;
}