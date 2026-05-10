using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using InvestmentControl.Application.NSI.DTOs;
using InvestmentControl.Application.NSI.Queries;

namespace InvestmentControl.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class NsiController : ControllerBase
{
    private readonly IMediator _mediator;
    public NsiController(IMediator mediator) => _mediator = mediator;

    [HttpGet("statuses")]
    public async Task<ActionResult<List<StatusDto>>> GetStatuses()
        => Ok(await _mediator.Send(new GetStatusesQuery()));

    [HttpGet("categories")]
    public async Task<ActionResult<List<CategoryDto>>> GetCategories()
        => Ok(await _mediator.Send(new GetCategoriesQuery()));

    [HttpGet("directions")]
    public async Task<ActionResult<List<DirectionDto>>> GetDirections()
        => Ok(await _mediator.Send(new GetDirectionsQuery()));

    [HttpGet("departments")]
    public async Task<ActionResult<List<DepartmentDto>>> GetDepartments()
        => Ok(await _mediator.Send(new GetDepartmentsQuery()));
}
