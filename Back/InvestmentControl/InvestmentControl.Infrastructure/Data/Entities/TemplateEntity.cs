namespace InvestmentControl.Infrastructure.Data.Entities;

public class TemplateEntity
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public int UserId { get; set; }

    public string FiltersJson { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}
