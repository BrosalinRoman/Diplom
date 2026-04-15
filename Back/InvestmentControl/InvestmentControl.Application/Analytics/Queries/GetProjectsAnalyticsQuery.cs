using InvestmentControl.Application.Analytics.DTOs;
using InvestmentControl.Application.Common.Helpers;
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
        ValidationHelper.EnsurePositiveIds(request.DirectionIds, nameof(request.DirectionIds));
        ValidationHelper.EnsurePositiveIds(request.DepartmentIds, nameof(request.DepartmentIds));
        ValidationHelper.EnsurePositiveIds(request.StatusIds, nameof(request.StatusIds));
        ValidationHelper.EnsurePositiveIds(request.ExcludedProjectIds, nameof(request.ExcludedProjectIds));

        // Проверка обязательного CategoryId
        if (request.CategoryId <= 0)
            throw new ArgumentException("CategoryId обязателен и должен быть положительным.");

        // Проверка рангов на отрицательные значения
        if (request.RankMin.HasValue && request.RankMin < 0)
            throw new ArgumentException("RankMin не может быть отрицательным.");
        if (request.RankMax.HasValue && request.RankMax < 0)
            throw new ArgumentException("RankMax не может быть отрицательным.");

        // Проверка диапазона Rank
        if (request.RankMin.HasValue && request.RankMax.HasValue && request.RankMin > request.RankMax)
            throw new ArgumentException("RankMin не может быть больше RankMax.");

        List<int>? allowedProjectIds = null;
        if (_currentUser.Role == "Applicant")
            allowedProjectIds = await _projectReadRepository.GetProjectIdsByCreatorAsync(_currentUser.UserId, cancellationToken);

        var projects = await _projectReadRepository.GetFilteredProjectsAsync(
            request.CategoryId,
            request.DirectionIds,
            request.DepartmentIds,
            request.StatusIds,
            request.RankMin,
            request.RankMax,
            allowedProjectIds,
            request.ExcludedProjectIds,
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
