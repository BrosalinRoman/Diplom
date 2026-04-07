// Application/Control/Queries/GetProgressReportsQuery.cs
using MediatR;
using InvestmentControl.Application.Control.DTOs;
using InvestmentControl.Domain.Interfaces;

namespace InvestmentControl.Application.Control.Queries;

public class GetProgressReportsQuery : IRequest<List<ProgressReportDto>>
{
    public int ProjectId { get; set; }
}

public class GetProgressReportsQueryHandler : IRequestHandler<GetProgressReportsQuery, List<ProgressReportDto>>
{
    private readonly IProgressReportRepository _progressReportRepository;

    public GetProgressReportsQueryHandler(IProgressReportRepository progressReportRepository)
    {
        _progressReportRepository = progressReportRepository;
    }

    public async Task<List<ProgressReportDto>> Handle(GetProgressReportsQuery request, CancellationToken cancellationToken)
    {
        var reports = await _progressReportRepository.GetByProjectIdAsync(request.ProjectId, cancellationToken);
        return reports.Select(r => new ProgressReportDto
        {
            Id = r.Id,
            Description = r.Description,
            ProgressPercentage = r.ProgressPercentage,
            ReportDate = r.ReportDate
        }).ToList();
    }
}
