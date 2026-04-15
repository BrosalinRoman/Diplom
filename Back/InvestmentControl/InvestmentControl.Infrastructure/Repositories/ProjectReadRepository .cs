using Microsoft.EntityFrameworkCore;
using InvestmentControl.Domain.Interfaces;
using InvestmentControl.Domain.ReadModels;
using InvestmentControl.Infrastructure.Data;

namespace InvestmentControl.Infrastructure.Repositories;

public class ProjectReadRepository : IProjectReadRepository
{
    private readonly ReadOnlyAppDbContext _readOnlyContext;
    private readonly ControlDbContext _controlContext;

    public ProjectReadRepository(ReadOnlyAppDbContext readOnlyContext, ControlDbContext controlContext)
    {
        _readOnlyContext = readOnlyContext;
        _controlContext = controlContext;
    }

    public async Task<List<ProjectReadModel>> GetFilteredProjectsAsync(
        int categoryId,
        List<int>? directionIds,
        List<int>? departmentIds,
        List<int>? statusIds,
        decimal? rankMin,
        decimal? rankMax,
        List<int>? allowedProjectIds,
        List<int>? excludedProjectIds,
        string? search,
        CancellationToken cancellationToken)
    {
        var query = _readOnlyContext.Projects.AsQueryable();

        query = query.Where(p => p.CategoryId == categoryId);

        if (directionIds != null && directionIds.Any())
            query = query.Where(p => directionIds.Contains(p.DirectionId));
        if (departmentIds != null && departmentIds.Any())
            query = query.Where(p => departmentIds.Contains(p.DepartmentId));
        if (statusIds != null && statusIds.Any())
            query = query.Where(p => p.StatusId.HasValue && statusIds.Contains(p.StatusId.Value));
        if (rankMin.HasValue)
            query = query.Where(p => p.Rank >= rankMin.Value);
        if (rankMax.HasValue)
            query = query.Where(p => p.Rank <= rankMax.Value);
        if (allowedProjectIds != null && allowedProjectIds.Any())
            query = query.Where(p => allowedProjectIds.Contains(p.Id));
        if (excludedProjectIds != null && excludedProjectIds.Any())
            query = query.Where(p => !excludedProjectIds.Contains(p.Id));
        if (!string.IsNullOrEmpty(search))
            query = query.Where(p => p.Name.Contains(search));

        var projects = await query.ToListAsync(cancellationToken);
        if (!projects.Any())
            return new List<ProjectReadModel>();

        var projectIdsList = projects.Select(p => p.Id).ToList();
        var characteristicValues = await _readOnlyContext.ProjectCharacteristicValues
            .Where(pcv => projectIdsList.Contains(pcv.ProjectId))
            .ToListAsync(cancellationToken);

        var categoryCharacteristics = await _readOnlyContext.CategoryCharacteristics
            .Where(cc => cc.CategoryId == categoryId)
            .ToListAsync(cancellationToken);
        var characteristicIds = categoryCharacteristics.Select(cc => cc.CharacteristicId).Distinct().ToList();
        var characteristics = await _readOnlyContext.Characteristics
            .Where(ch => characteristicIds.Contains(ch.Id))
            .ToDictionaryAsync(ch => ch.Id, ch => ch.Name, cancellationToken);

        foreach (var p in projects)
        {
            var dict = new Dictionary<string, decimal?>();
            var values = characteristicValues.Where(v => v.ProjectId == p.Id);
            foreach (var val in values)
            {
                var cc = categoryCharacteristics.FirstOrDefault(cc => cc.Id == val.CategoryCharacteristicId);
                if (cc != null && characteristics.TryGetValue(cc.CharacteristicId, out var charName))
                {
                    dict[charName] = val.Value;
                }
            }
            p.Characteristics = dict;
        }

        return projects;
    }

    public async Task<List<int>> GetProjectIdsByCreatorAsync(int userId, CancellationToken cancellationToken)
    {
        return await _readOnlyContext.Projects
            .Where(p => p.CreatedByUserId == userId)
            .Select(p => p.Id)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<DepartmentSummaryReadModel>> GetDepartmentSummaryAsync(
        List<int>? departmentIds,
        DateTime? dateFrom,
        DateTime? dateTo,
        List<int>? statusIds,
        List<int>? directionIds,
        List<int>? categoryIds,
        CancellationToken cancellationToken)
    {
        var query = _readOnlyContext.Projects.AsQueryable();

        if (departmentIds != null && departmentIds.Any())
            query = query.Where(p => departmentIds.Contains(p.DepartmentId));

        // Фильтр по дате публикации
        if (dateFrom.HasValue)
            query = query.Where(p => p.PublishedAt >= dateFrom.Value);
        if (dateTo.HasValue)
            query = query.Where(p => p.PublishedAt <= dateTo.Value);

        if (statusIds != null && statusIds.Any())
            query = query.Where(p => p.StatusId.HasValue && statusIds.Contains(p.StatusId.Value));
        if (directionIds != null && directionIds.Any())
            query = query.Where(p => directionIds.Contains(p.DirectionId));
        if (categoryIds != null && categoryIds.Any())
            query = query.Where(p => categoryIds.Contains(p.CategoryId));

        var projects = await query.ToListAsync(cancellationToken);
        if (!projects.Any())
            return new List<DepartmentSummaryReadModel>();

        var departmentNames = await _readOnlyContext.Departments
            .ToDictionaryAsync(d => d.Id, d => d.Name, cancellationToken);

        // Используем поле Budget из таблицы проектов (без дополнительных характеристик)
        var grouped = projects.GroupBy(p => p.DepartmentId)
            .Select(g => new DepartmentSummaryReadModel
            {
                DepartmentId = g.Key,
                DepartmentName = departmentNames.GetValueOrDefault(g.Key, "Неизвестно"),
                ProjectCount = g.Count(),
                TotalBudget = g.Sum(p => p.Budget ?? 0)
            })
            .OrderBy(d => d.DepartmentName)
            .ToList();

        return grouped;
    }

    public async Task<List<ControlProjectReadModel>> GetControlProjectsAsync(
        string? search,
        List<int>? directionIds,
        List<int>? departmentIds,
        List<int>? categoryIds,
        List<int>? projectIds,
        string? sort,
        CancellationToken cancellationToken)
    {
        // Получаем ID статусов "Активен" и "Завершен"
        var activeStatusId = await _readOnlyContext.Statuses
            .Where(s => s.Name == "Активен")
            .Select(s => s.Id)
            .FirstOrDefaultAsync(cancellationToken);
        var completedStatusId = await _readOnlyContext.Statuses
            .Where(s => s.Name == "Завершен")
            .Select(s => s.Id)
            .FirstOrDefaultAsync(cancellationToken);

        var allowedStatusIds = new List<int>();
        if (activeStatusId != 0) allowedStatusIds.Add(activeStatusId);
        if (completedStatusId != 0) allowedStatusIds.Add(completedStatusId);

        var query = _readOnlyContext.Projects.Where(p => allowedStatusIds.Contains(p.StatusId ?? 0));

        if (!string.IsNullOrEmpty(search))
            query = query.Where(p => p.Name.Contains(search));
        if (directionIds != null && directionIds.Any())
            query = query.Where(p => directionIds.Contains(p.DirectionId));
        if (departmentIds != null && departmentIds.Any())
            query = query.Where(p => departmentIds.Contains(p.DepartmentId));
        if (categoryIds != null && categoryIds.Any())
            query = query.Where(p => categoryIds.Contains(p.CategoryId));
        if (projectIds != null && projectIds.Any())
            query = query.Where(p => projectIds.Contains(p.Id));

        var projects = await query.ToListAsync(cancellationToken);
        if (!projects.Any())
            return new List<ControlProjectReadModel>();

        var categoryNames = await _readOnlyContext.Categories
            .ToDictionaryAsync(c => c.Id, c => c.Name, cancellationToken);
        var directionNames = await _readOnlyContext.Directions
            .ToDictionaryAsync(d => d.Id, d => d.Name, cancellationToken);
        var departmentNames = await _readOnlyContext.Departments
            .ToDictionaryAsync(d => d.Id, d => d.Name, cancellationToken);

        var investments = await _controlContext.Investments
            .Where(i => i.ActualAmount.HasValue)
            .GroupBy(i => i.ProjectId)
            .Select(g => new { ProjectId = g.Key, Sum = g.Sum(i => i.ActualAmount.Value) })
            .ToDictionaryAsync(x => x.ProjectId, x => x.Sum, cancellationToken);

        var reports = await _controlContext.ProgressReports
            .GroupBy(r => r.ProjectId)
            .Select(g => new { ProjectId = g.Key, MaxProgress = g.Max(r => r.ProgressPercentage) })
            .ToDictionaryAsync(x => x.ProjectId, x => x.MaxProgress, cancellationToken);

        var result = projects.Select(p => new ControlProjectReadModel
        {
            Id = p.Id,
            Name = p.Name,
            Category = categoryNames.GetValueOrDefault(p.CategoryId, string.Empty),
            Direction = directionNames.GetValueOrDefault(p.DirectionId, string.Empty),
            Department = departmentNames.GetValueOrDefault(p.DepartmentId, string.Empty),
            Budget = p.Budget ?? 0,
            Invested = investments.GetValueOrDefault(p.Id, 0),
            Progress = reports.GetValueOrDefault(p.Id, 0),
            StartDate = p.PublishedAt ?? p.CreatedAt
        }).ToList();

        // Валидация параметра sort – расширенный список
        var allowedSortValues = new[] { "date_desc", "name_asc", "name_desc", "progress_desc", "progress_asc", "budget_desc", "budget_asc", "invested_desc", "invested_asc" };
        if (!string.IsNullOrEmpty(sort) && !allowedSortValues.Contains(sort))
            throw new ArgumentException($"Недопустимое значение sort. Допустимы: {string.Join(", ", allowedSortValues)}");

        result = sort switch
        {
            "name_asc" => result.OrderBy(r => r.Name).ToList(),
            "name_desc" => result.OrderByDescending(r => r.Name).ToList(),
            "progress_desc" => result.OrderByDescending(r => r.Progress).ToList(),
            "progress_asc" => result.OrderBy(r => r.Progress).ToList(),
            "budget_desc" => result.OrderByDescending(r => r.Budget).ToList(),
            "budget_asc" => result.OrderBy(r => r.Budget).ToList(),
            "invested_desc" => result.OrderByDescending(r => r.Invested).ToList(),
            "invested_asc" => result.OrderBy(r => r.Invested).ToList(),
            _ => result.OrderByDescending(r => r.StartDate).ToList() // date_desc
        };

        return result;
    }

    // Вспомогательные методы
    public async Task<bool> ExistsAsync(int projectId, CancellationToken cancellationToken)
    {
        return await _readOnlyContext.Projects.AnyAsync(p => p.Id == projectId, cancellationToken);
    }

    public async Task<string?> GetStatusAsync(int projectId, CancellationToken cancellationToken)
    {
        var project = await _readOnlyContext.Projects
            .Include(p => p.Status)
            .FirstOrDefaultAsync(p => p.Id == projectId, cancellationToken);
        return project?.Status?.Name;
    }

    public async Task<int?> GetCreatorUserIdAsync(int projectId, CancellationToken cancellationToken)
    {
        var project = await _readOnlyContext.Projects
            .Where(p => p.Id == projectId)
            .Select(p => p.CreatedByUserId)
            .FirstOrDefaultAsync(cancellationToken);
        return project == 0 ? null : project;
    }

    public async Task<DateTime?> GetPublishedAtAsync(int projectId, CancellationToken cancellationToken)
    {
        var project = await _readOnlyContext.Projects
            .Where(p => p.Id == projectId)
            .Select(p => p.PublishedAt)
            .FirstOrDefaultAsync(cancellationToken);
        return project;
    }

    public async Task<decimal?> GetBudgetAsync(int projectId, CancellationToken cancellationToken)
    {
        var project = await _readOnlyContext.Projects
            .Where(p => p.Id == projectId)
            .Select(p => p.Budget)
            .FirstOrDefaultAsync(cancellationToken);
        return project;
    }

    public async Task<int?> GetStatusIdByNameAsync(string name, CancellationToken cancellationToken)
    {
        var status = await _readOnlyContext.Statuses
            .FirstOrDefaultAsync(s => s.Name == name, cancellationToken);
        return status?.Id;
    }

    public async Task<List<int>> GetAllStatusIdsAsync(CancellationToken cancellationToken)
    {
        return await _readOnlyContext.Statuses.Select(s => s.Id).ToListAsync(cancellationToken);
    }
}