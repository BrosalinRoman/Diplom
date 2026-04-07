namespace InvestmentControl.Domain.ReadModels;

public class DepartmentReadModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}