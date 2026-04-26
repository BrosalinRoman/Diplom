using System.Text.Json;

namespace InvestmentControl.Domain.Models;

public class Template
{
    public int Id { get; private set; }
    public string Name { get; private set; }
    public int UserId { get; private set; }
    public string FiltersJson { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    public Template(string name, int userId, string filtersJson)
    {
        ValidateName(name);
        ValidateFiltersJson(filtersJson);

        Name = name;
        UserId = userId;
        FiltersJson = filtersJson;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = null;
    }

    public Template(int id, string name, int userId, string filtersJson, DateTime createdAt, DateTime? updatedAt)
    {
        Id = id;
        Name = name;
        UserId = userId;
        FiltersJson = filtersJson;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
    }

    public void Update(string name, string filtersJson)
    {
        ValidateName(name);
        ValidateFiltersJson(filtersJson);

        Name = name;
        FiltersJson = filtersJson;
        UpdatedAt = DateTime.UtcNow;
    }

    private static void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Имя шаблона обязательно.", nameof(name));
        if (name.Length > 100)
            throw new ArgumentException("Имя шаблона не должно превышать 100 символов.", nameof(name));
    }

    public static void ValidateFiltersJson(string json)
    {
        if (string.IsNullOrWhiteSpace(json))
            throw new ArgumentException("FiltersJson обязателен.", nameof(json));

        try
        {
            using var doc = JsonDocument.Parse(json);
            if (!doc.RootElement.TryGetProperty("categoryId", out _))
                throw new ArgumentException("FiltersJson должен содержать поле categoryId.");

            // Дополнительно: проверка обязательных полей DirectionIds, DepartmentIds, StatusIds, SelectedFields
            // (по новым требованиям – они не могут быть null или пустыми)
            ValidateRequiredArrayField(doc.RootElement, "directionIds", "DirectionIds");
            ValidateRequiredArrayField(doc.RootElement, "departmentIds", "DepartmentIds");
            ValidateRequiredArrayField(doc.RootElement, "statusIds", "StatusIds");
            ValidateRequiredArrayField(doc.RootElement, "selectedFields", "SelectedFields");
        }
        catch (JsonException)
        {
            throw new ArgumentException("FiltersJson должен быть валидным JSON.");
        }
    }

    private static void ValidateRequiredArrayField(JsonElement root, string fieldName, string displayName)
    {
        if (!root.TryGetProperty(fieldName, out var arrayElement))
            throw new ArgumentException($"FiltersJson должен содержать поле {displayName}.");
        if (arrayElement.ValueKind != JsonValueKind.Array || arrayElement.GetArrayLength() == 0)
            throw new ArgumentException($"Поле {displayName} в FiltersJson должно быть массивом, содержащим хотя бы один элемент.");
    }
}