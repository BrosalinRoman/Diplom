namespace InvestmentControl.Domain.ReadModels;

public class ProjectCharacteristicValueReadModel
{
    public int Id { get; set; }
    public int ProjectId { get; set; }
    public int CategoryCharacteristicId { get; set; }
    public decimal? Value { get; set; }
    public int? Score { get; set; }

    // Навигация
    public ProjectReadModel? Project { get; set; }
    public CategoryCharacteristicReadModel? CategoryCharacteristic { get; set; }
}