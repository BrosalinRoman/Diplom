using InvestmentControl.API.DTOs.Requests;
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
    public async Task<ActionResult<List<ControlProjectDto>>> GetProjects([FromQuery] ControlProjectsRequest request)
    {
        var query = new GetControlProjectsQuery
        {
            Search = request.Search,
            DirectionIds = request.DirectionIds,
            DepartmentIds = request.DepartmentIds,
            CategoryIds = request.CategoryIds,
            Sort = request.Sort
        };
        var projects = await _mediator.Send(query);
        return Ok(projects);
    }

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
        return Ok(id);
    }

    [HttpPut("investments/{id}")]
    public async Task<IActionResult> UpdateInvestment(int id, [FromBody] InvestmentRequest request)
    {
        var command = new UpdateInvestmentCommand
        {
            Id = id,
            PlannedAmount = request.PlannedAmount,
            PlannedDate = request.PlannedDate,
            ActualAmount = request.ActualAmount,
            ActualDate = request.ActualDate
        };
        await _mediator.Send(command);
        return NoContent();
    }

    [HttpDelete("investments/{id}")]
    public async Task<IActionResult> DeleteInvestment(int id)
    {
        await _mediator.Send(new DeleteInvestmentCommand { Id = id });
        return NoContent();
    }

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
        return Ok(id);
    }

    [HttpPut("costs/{id}")]
    public async Task<IActionResult> UpdateCost(int id, [FromBody] CostRequest request)
    {
        var command = new UpdateCostCommand
        {
            Id = id,
            Amount = request.Amount,
            Description = request.Description,
            Responsible = request.Responsible,
            Date = request.Date
        };
        await _mediator.Send(command);
        return NoContent();
    }

    [HttpDelete("costs/{id}")]
    public async Task<IActionResult> DeleteCost(int id)
    {
        await _mediator.Send(new DeleteCostCommand { Id = id });
        return NoContent();
    }

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
        return Ok(id);
    }

    [HttpPut("progress-reports/{id}")]
    public async Task<IActionResult> UpdateProgressReport(int id, [FromBody] ProgressReportRequest request)
    {
        var command = new UpdateProgressReportCommand
        {
            Id = id,
            Description = request.Description,
            ProgressPercentage = request.ProgressPercentage
        };
        await _mediator.Send(command);
        return NoContent();
    }

    [HttpDelete("progress-reports/{id}")]
    public async Task<IActionResult> DeleteProgressReport(int id)
    {
        await _mediator.Send(new DeleteProgressReportCommand { Id = id });
        return NoContent();
    }
}
