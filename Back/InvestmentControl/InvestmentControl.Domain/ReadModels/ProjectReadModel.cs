namespace InvestmentControl.Domain.ReadModels;

public class ProjectReadModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Goal { get; set; }
    public int CategoryId { get; set; }
    public int DirectionId { get; set; }
    public int DepartmentId { get; set; }
    public int? StatusId { get; set; }
    public int CreatedByUserId { get; set; }
    public int? ResponsibleUserId { get; set; }
    public decimal? Rank { get; set; }
    public decimal? Budget { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? PublishedAt { get; set; }

    // Словарь для характеристик (вычисляемое, не в БД)
    public Dictionary<string, decimal?> Characteristics { get; set; } = new();

    // Навигационные свойства (для Include)
    public CategoryReadModel? Category { get; set; }
    public DirectionReadModel? Direction { get; set; }
    public DepartmentReadModel? Department { get; set; }
    public StatusReadModel? Status { get; set; }
}