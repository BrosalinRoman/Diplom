namespace InvestmentControl.API.DTOs.Requests;

public class SaveTemplateRequest
{
    public string Name { get; set; } = string.Empty;
    public string FiltersJson { get; set; } = string.Empty;
}
