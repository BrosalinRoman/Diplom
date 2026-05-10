namespace InvestmentControl.Application.Control.DTOs;

public class ProjectInfoDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Budget { get; set; }
}