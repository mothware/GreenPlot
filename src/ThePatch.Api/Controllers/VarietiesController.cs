using ThePatch.Application.Features.Varieties.Commands;
using ThePatch.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ThePatch.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/varieties")]
public class VarietiesController : ControllerBase
{
    private readonly IMediator _mediator;

    public VarietiesController(IMediator mediator) => _mediator = mediator;

    /// <summary>
    /// Add a variety (and optionally a new plant) directly from a seed packet.
    /// Creates a SeedLot for inventory tracking by default.
    /// </summary>
    [HttpPost("from-packet")]
    public async Task<IActionResult> AddFromPacket(
        [FromBody] AddFromPacketRequest request,
        CancellationToken ct)
    {
        var command = new AddVarietyFromPacketCommand(
            request.ExistingPlantId,
            request.NewPlantCommonName,
            request.NewPlantFamily,
            request.NewPlantCategory,
            request.VarietyName,
            request.Supplier,
            request.PacketYear,
            request.DaysToMaturity,
            request.DaysToGerminate,
            request.SowingDepthInches,
            request.SpacingInches,
            request.DiseaseResistance,
            request.FlavorNotes,
            request.ColorNotes,
            request.Notes,
            request.CreateSeedLot,
            request.PacketSeedCount,
            request.PurchaseDate
        );

        var result = await _mediator.Send(command, ct);
        return Created($"/api/plants/{result.Variety.PlantId}/varieties/{result.Variety.Id}", result);
    }
}

public record AddFromPacketRequest(
    Guid? ExistingPlantId,
    string? NewPlantCommonName,
    string? NewPlantFamily,
    PlantCategory? NewPlantCategory,
    string VarietyName,
    string? Supplier,
    int? PacketYear,
    int? DaysToMaturity,
    int? DaysToGerminate,
    decimal? SowingDepthInches,
    decimal? SpacingInches,
    string? DiseaseResistance,
    string? FlavorNotes,
    string? ColorNotes,
    string? Notes,
    bool CreateSeedLot = true,
    int? PacketSeedCount = null,
    DateOnly? PurchaseDate = null
);
