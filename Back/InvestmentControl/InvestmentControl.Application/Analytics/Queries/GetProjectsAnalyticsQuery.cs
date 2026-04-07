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
    public List<int>? ProjectIds { get; set; }
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
        // Определяем, какие проекты видит пользователь
        List<int>? projectIds = request.ProjectIds;
        if (projectIds == null || !projectIds.Any())
        {
            // Если роль Applicant - только свои проекты
            if (_currentUser.Role == "Applicant")
                projectIds = await _projectReadRepository.GetProjectIdsByCreatorAsync(_currentUser.UserId, cancellationToken);
            // Для Investor projectIds остаётся null (означает все проекты)
        }

        // Получаем проекты с фильтрацией (возвращает List<ProjectReadModel>)
        var projects = await _projectReadRepository.GetFilteredProjectsAsync(
            request.CategoryId,
            request.DirectionIds,
            request.DepartmentIds,
            request.StatusIds,
            request.RankMin,
            request.RankMax,
            projectIds,
            request.Search,
            cancellationToken);

        // Маппим read-модели в DTO
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
