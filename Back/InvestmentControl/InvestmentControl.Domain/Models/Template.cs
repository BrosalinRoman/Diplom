namespace InvestmentControl.Domain.Models;

/// <summary>
/// Доменная модель шаблона фильтров аналитики.
/// Не зависит от ORM.
/// </summary>
public class Template
{
    public int Id { get; private set; }
    public string Name { get; private set; }
    public int UserId { get; private set; }
    public string FiltersJson { get; private set; } // JSONB в БД
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    // Конструктор для создания нового шаблона
    public Template(string name, int userId, string filtersJson)
    {
        Name = name;
        UserId = userId;
        FiltersJson = filtersJson;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = null;
    }

    // Конструктор для восстановления из хранилища
    public Template(int id, string name, int userId, string filtersJson, DateTime createdAt, DateTime? updatedAt)
    {
        Id = id;
        Name = name;
        UserId = userId;
        FiltersJson = filtersJson;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
    }

    // Метод для обновления шаблона
    public void Update(string name, string filtersJson)
    {
        Name = name;
        FiltersJson = filtersJson;
        UpdatedAt = DateTime.UtcNow;
    }
}