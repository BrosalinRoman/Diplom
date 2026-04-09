using InvestmentControl.Application.Analytics.DTOs;
using InvestmentControl.Application.Common.Interfaces;
using InvestmentControl.Domain.Interfaces;
using MediatR;

namespace InvestmentControl.Application.Analytics.Queries;

public class GetProjectsAnalyticsQuery : IRequest<List<ProjectAnalyticsDto>>
{
    public int CategoryId { get; set; }
    public List<int>? DirectionIds { get; set; }
    public List<int>? DepartmentIds { get; set; }
    public List<int>? StatusIds { get; set; }
    public decimal? RankMin { get; set; }
    public decimal? RankMax { get; set; }
    public List<int>? ExcludedProjectIds { get; set; } // ИЗМЕНЕНО: исключаемые ID
    public List<string>? SelectedFields { get; set; }
    public string? Search { get; set; }
}

public class GetProjectsAnalyticsQueryHandler : IRequestHandler<GetProjectsAnalyticsQuery, List<ProjectAnalyticsDto>>
{
    private readonly IProjectReadRepository _projectReadRepository;
    private readonly ICurrentUser _currentUser;

    public GetProjectsAnalyticsQueryHandler(IProjectReadRepository projectReadRepository, ICurrentUser currentUser)
    {
        _projectReadRepository = projectReadRepository;
        _currentUser = currentUser;
    }

    public async Task<List<ProjectAnalyticsDto>> Handle(GetProjectsAnalyticsQuery request, CancellationToken cancellationToken)
    {
        // Определяем, какие проекты видит пользователь (только свои для Applicant)
        List<int>? allowedProjectIds = null;
        if (_currentUser.Role == "Applicant")
        {
            allowedProjectIds = await _projectReadRepository.GetProjectIdsByCreatorAsync(_currentUser.UserId, cancellationToken);
        }

        // Получаем проекты с фильтрацией
        var projects = await _projectReadRepository.GetFilteredProjectsAsync(
            request.CategoryId,
            request.DirectionIds,
            request.DepartmentIds,
            request.StatusIds,
            request.RankMin,
            request.RankMax,
            allowedProjectIds,          // проекты, которые пользователь может видеть
            request.ExcludedProjectIds, // проекты, которые нужно исключить
            request.Search,
            cancellationToken);

        var result = projects.Select(p => new ProjectAnalyticsDto
        {
            Id = p.Id,
            Name = p.Name,
            Rank = p.Rank,
            Characteristics = p.Characteristics
                .Where(c => request.SelectedFields == null || request.SelectedFields.Contains(c.Key))
                .ToDictionary(c => c.Key, c => c.Value)
        }).ToList();

        return result;
    }
}
