namespace InvestmentControl.Domain.ReadModels;

public class CharacteristicReadModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Unit { get; set; }
}