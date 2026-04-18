using GreenPlot.Application.Features.Beds.Commands;
using GreenPlot.Application.Features.Beds.Queries;
using GreenPlot.Application.Features.Gardens.Commands;
using GreenPlot.Application.Features.Gardens.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GreenPlot.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/gardens")]
public class GardensController : ControllerBase
{
    private readonly IMediator _mediator;

    public GardensController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    public async Task<IActionResult> GetGardens(CancellationToken ct)
    {
        var gardens = await _mediator.Send(new GetGardensQuery(), ct);
        return Ok(gardens);
    }

    [HttpPost]
    public async Task<IActionResult> CreateGarden([FromBody] CreateGardenCommand command, CancellationToken ct)
    {
        var garden = await _mediator.Send(command, ct);
        return CreatedAtAction(nameof(GetGardens), new { id = garden.Id }, garden);
    }

    [HttpGet("{gardenId:guid}/beds/{bedId:guid}")]
    public async Task<IActionResult> GetBed(Guid bedId, CancellationToken ct)
    {
        var bed = await _mediator.Send(new GetBedDetailQuery(bedId), ct);
        return Ok(bed);
    }

    [HttpPost("{gardenId:guid}/beds")]
    public async Task<IActionResult> CreateBed(Guid gardenId, [FromBody] CreateBedRequest request, CancellationToken ct)
    {
        var command = new CreateBedCommand(
            gardenId, request.Name, request.Type, request.Shape,
            request.WidthFeet, request.LengthFeet, request.GridCellSizeFeet,
            request.SunExposure, request.SoilNotes);
        var bed = await _mediator.Send(command, ct);
        return CreatedAtAction(nameof(GetBed), new { gardenId, bedId = bed.Id }, bed);
    }
}

public record CreateBedRequest(
    string Name,
    GreenPlot.Domain.Enums.BedType Type,
    GreenPlot.Domain.Enums.BedShape Shape,
    decimal WidthFeet,
    decimal LengthFeet,
    decimal GridCellSizeFeet = 1m,
    GreenPlot.Domain.Enums.SunRequirement? SunExposure = null,
    string? SoilNotes = null
);
