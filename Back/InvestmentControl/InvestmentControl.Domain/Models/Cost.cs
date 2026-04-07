namespace InvestmentControl.Domain.Models;

public class Cost
{
    public int Id { get; private set; }
    public int ProjectId { get; private set; }
    public decimal Amount { get; private set; }
    public string Description { get; private set; }
    public string Responsible { get; private set; }
    public DateTime Date { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public Cost(int projectId, decimal amount, string description, string responsible, DateTime date)
    {
        ProjectId = projectId;
        Amount = amount;
        Description = description;
        Responsible = responsible;
        Date = date;
        CreatedAt = DateTime.UtcNow;
    }

    public Cost(int id, int projectId, decimal amount, string description, string responsible, DateTime date, DateTime createdAt)
    {
        Id = id;
        ProjectId = projectId;
        Amount = amount;
        Description = description;
        Responsible = responsible;
        Date = date;
        CreatedAt = createdAt;
    }

    public void Update(decimal amount, string description, string responsible, DateTime date)
    {
        Amount = amount;
        Description = description;
        Responsible = responsible;
        Date = date;
    }
}
