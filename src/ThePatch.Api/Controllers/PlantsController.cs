using ThePatch.Application.Features.Plants.Commands;
using ThePatch.Application.Features.Plants.Queries;
using ThePatch.Application.Features.Varieties.Queries;
using ThePatch.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ThePatch.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/plants")]
public class PlantsController : ControllerBase
{
    private readonly IMediator _mediator;

    public PlantsController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    public async Task<IActionResult> GetPlants(
        [FromQuery] string? search,
        [FromQuery] PlantCategory? category,
        [FromQuery] PlantLifecycle? lifecycle,
        [FromQuery] SunRequirement? sunRequirement,
        [FromQuery] int? daysToMaturityMin,
        [FromQuery] int? daysToMaturityMax,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 25,
        CancellationToken ct = default)
    {
        var result = await _mediator.Send(new GetPlantsQuery(
            search, category, lifecycle, sunRequirement,
            daysToMaturityMin, daysToMaturityMax, page, pageSize), ct);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetPlant(Guid id, CancellationToken ct)
    {
        var plant = await _mediator.Send(new GetPlantByIdQuery(id), ct);
        return Ok(plant);
    }

    [HttpGet("{id:guid}/varieties")]
    public async Task<IActionResult> GetVarieties(Guid id, CancellationToken ct)
    {
        var varieties = await _mediator.Send(new GetVarietiesByPlantQuery(id), ct);
        return Ok(varieties);
    }

    [HttpPost]
    public async Task<IActionResult> CreatePlant([FromBody] CreatePlantCommand command, CancellationToken ct)
    {
        var plant = await _mediator.Send(command, ct);
        return CreatedAtAction(nameof(GetPlant), new { id = plant.Id }, plant);
    }
}
