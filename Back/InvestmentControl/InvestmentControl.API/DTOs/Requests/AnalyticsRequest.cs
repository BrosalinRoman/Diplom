using System.ComponentModel.DataAnnotations;

namespace InvestmentControl.API.DTOs.Requests;

public class AnalyticsRequest
{
    [Required(ErrorMessage = "CategoryId обязателен")]
    public int CategoryId { get; set; }

    public List<int>? DirectionIds { get; set; }
    public List<int>? DepartmentIds { get; set; }
    public List<int>? StatusIds { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "RankMin не может быть отрицательным")]
    public decimal? RankMin { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "RankMax не может быть отрицательным")]
    public decimal? RankMax { get; set; }

    public List<int>? ProjectIds { get; set; } // Теперь это список ID для исключения
    public List<string>? SelectedFields { get; set; }
    public string? Search { get; set; }
}
