using InvestmentControl.API.DTOs.Requests;
using InvestmentControl.Application.Common.DTOs;
using InvestmentControl.Application.Control.Commands;
using InvestmentControl.Application.Control.DTOs;
using InvestmentControl.Application.Control.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InvestmentControl.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ControlController : ControllerBase
{
    private readonly IMediator _mediator;

    public ControlController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("projects")]
    public async Task<ActionResult<PagedResponse<ControlProjectDto>>> GetProjects([FromQuery] ControlProjectsRequest request)
    {
        var query = new GetControlProjectsQuery
        {
            Search = request.Search,
            DirectionIds = request.DirectionIds,
            DepartmentIds = request.DepartmentIds,
            CategoryIds = request.CategoryIds,
            StatusIds = request.StatusIds,
            Sort = request.Sort,
            DateFrom = request.DateFrom,
            DateTo = request.DateTo,
            Page = request.Page,
            PageSize = request.PageSize
        };
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("projects/{projectId}/info")]
    public async Task<ActionResult<ProjectInfoDto>> GetProjectInfo(int projectId)
    {
        var info = await _mediator.Send(new GetProjectInfoQuery { ProjectId = projectId });
        return Ok(info);
    }

    // investments
    [HttpGet("projects/{projectId}/investments")]
    public async Task<ActionResult<List<InvestmentDto>>> GetInvestments(int projectId)
    {
        var investments = await _mediator.Send(new GetInvestmentsQuery { ProjectId = projectId });
        return Ok(investments);
    }

    [HttpPost("projects/{projectId}/investments")]
    public async Task<ActionResult<int>> AddInvestment(int projectId, [FromBody] InvestmentRequest request)
    {
        var command = new AddInvestmentCommand
        {
            ProjectId = projectId,
            PlannedAmount = request.PlannedAmount,
            PlannedDate = request.PlannedDate,
            ActualAmount = request.ActualAmount,
            ActualDate = request.ActualDate
        };
        var id = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetInvestments), new { projectId }, id);
    }

    [HttpPut("investments/{id}")]
    public async Task<ActionResult<InvestmentDto>> UpdateInvestment(int id, [FromBody] InvestmentRequest request)
    {
        var command = new UpdateInvestmentCommand
        {
            Id = id,
            PlannedAmount = request.PlannedAmount,
            PlannedDate = request.PlannedDate,
            ActualAmount = request.ActualAmount,
            ActualDate = request.ActualDate
        };
        var investment = await _mediator.Send(command);
        return Ok(investment);
    }

    [HttpDelete("investments/{id}")]
    public async Task<IActionResult> DeleteInvestment(int id)
    {
        await _mediator.Send(new DeleteInvestmentCommand { Id = id });
        return NoContent();
    }

    // costs
    [HttpGet("projects/{projectId}/costs")]
    public async Task<ActionResult<List<CostDto>>> GetCosts(int projectId)
    {
        var costs = await _mediator.Send(new GetCostsQuery { ProjectId = projectId });
        return Ok(costs);
    }

    [HttpPost("projects/{projectId}/costs")]
    public async Task<ActionResult<int>> AddCost(int projectId, [FromBody] CostRequest request)
    {
        var command = new AddCostCommand
        {
            ProjectId = projectId,
            Amount = request.Amount,
            Description = request.Description,
            Responsible = request.Responsible,
            Date = request.Date
        };
        var id = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetCosts), new { projectId }, id);
    }

    [HttpPut("costs/{id}")]
    public async Task<ActionResult<CostDto>> UpdateCost(int id, [FromBody] CostRequest request)
    {
        var command = new UpdateCostCommand
        {
            Id = id,
            Amount = request.Amount,
            Description = request.Description,
            Responsible = request.Responsible,
            Date = request.Date
        };
        var cost = await _mediator.Send(command);
        return Ok(cost);
    }

    [HttpDelete("costs/{id}")]
    public async Task<IActionResult> DeleteCost(int id)
    {
        await _mediator.Send(new DeleteCostCommand { Id = id });
        return NoContent();
    }

    // progress reports
    [HttpGet("projects/{projectId}/progress-reports")]
    public async Task<ActionResult<List<ProgressReportDto>>> GetProgressReports(int projectId)
    {
        var reports = await _mediator.Send(new GetProgressReportsQuery { ProjectId = projectId });
        return Ok(reports);
    }

    [HttpPost("projects/{projectId}/progress-reports")]
    public async Task<ActionResult<int>> AddProgressReport(int projectId, [FromBody] ProgressReportRequest request)
    {
        var command = new AddProgressReportCommand
        {
            ProjectId = projectId,
            Description = request.Description,
            ProgressPercentage = request.ProgressPercentage
        };
        var id = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetProgressReports), new { projectId }, id);
    }

    [HttpPut("progress-reports/{id}")]
    public async Task<ActionResult<ProgressReportDto>> UpdateProgressReport(int id, [FromBody] ProgressReportRequest request)
    {
        var command = new UpdateProgressReportCommand
        {
            Id = id,
            Description = request.Description,
            ProgressPercentage = request.ProgressPercentage
        };
        var report = await _mediator.Send(command);
        return Ok(report);
    }

    [HttpDelete("progress-reports/{id}")]
    public async Task<IActionResult> DeleteProgressReport(int id)
    {
        await _mediator.Send(new DeleteProgressReportCommand { Id = id });
        return NoContent();
    }
}