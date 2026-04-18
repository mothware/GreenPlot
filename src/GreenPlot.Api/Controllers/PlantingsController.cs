using GreenPlot.Application.Features.Activities.Commands;
using GreenPlot.Application.Features.Plantings.Commands;
using GreenPlot.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GreenPlot.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/plantings")]
public class PlantingsController : ControllerBase
{
    private readonly IMediator _mediator;

    public PlantingsController(IMediator mediator) => _mediator = mediator;

    [HttpPost]
    public async Task<IActionResult> CreatePlanting([FromBody] CreatePlantingCommand command, CancellationToken ct)
    {
        var planting = await _mediator.Send(command, ct);
        return Created($"/api/plantings/{planting.Id}", planting);
    }

    [HttpPost("{plantingId:guid}/activities")]
    public async Task<IActionResult> LogActivity(
        Guid plantingId,
        [FromBody] LogActivityRequest request,
        CancellationToken ct)
    {
        var command = new LogActivityCommand(
            plantingId, request.Type, request.OccurredAt,
            request.Quantity, request.WeightGrams, request.Notes, request.PhotoUrls);
        var activity = await _mediator.Send(command, ct);
        return Created($"/api/plantings/{plantingId}/activities/{activity.Id}", activity);
    }
}

public record LogActivityRequest(
    ActivityType Type,
    DateTime OccurredAt,
    int? Quantity,
    decimal? WeightGrams,
    string? Notes,
    List<string>? PhotoUrls
);
