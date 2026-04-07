namespace InvestmentControl.Application.Control.DTOs;

public class ControlProjectDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Direction { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public decimal Budget { get; set; }
    public decimal Invested { get; set; } // сумма фактических инвестиций
    public decimal Progress { get; set; } // последний процент из отчётов
    public DateTime StartDate { get; set; } // дата начала инвестиций
}
