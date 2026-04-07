namespace InvestmentControl.Domain.ReadModels;

public class CategoryCharacteristicReadModel
{
    public int Id { get; set; }
    public int CategoryId { get; set; }
    public int CharacteristicId { get; set; }

    public CategoryReadModel? Category { get; set; }
    public CharacteristicReadModel? Characteristic { get; set; }
}