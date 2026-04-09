using System.ComponentModel.DataAnnotations;

namespace InvestmentControl.API.DTOs.Requests;

public class SaveTemplateRequest
{
    [Required(ErrorMessage = "Имя шаблона обязательно")]
    [MaxLength(100, ErrorMessage = "Имя не должно превышать 100 символов")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "FiltersJson обязателен")]
    public string FiltersJson { get; set; } = string.Empty;

    public int? TemplateId { get; set; }
}
