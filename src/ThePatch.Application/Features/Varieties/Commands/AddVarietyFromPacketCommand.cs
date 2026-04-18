using ThePatch.Application.Common.Interfaces;
using ThePatch.Application.DTOs;
using ThePatch.Domain.Entities;
using ThePatch.Domain.Enums;
using ThePatch.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ThePatch.Application.Features.Varieties.Commands;

/// <summary>
/// Records a variety the user has read from a physical seed packet and optionally
/// creates the parent plant if it doesn't exist in the catalog yet.
/// </summary>
public record AddVarietyFromPacketCommand(
    // Plant lookup — one of these should be provided
    Guid? ExistingPlantId,
    string? NewPlantCommonName,
    string? NewPlantFamily,
    PlantCategory? NewPlantCategory,

    // Variety data from the seed packet
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

    // Optional: create a SeedLot for the packet being entered
    bool CreateSeedLot = true,
    int? PacketSeedCount = null,
    DateOnly? PurchaseDate = null
) : IRequest<AddVarietyFromPacketResult>;

public record AddVarietyFromPacketResult(VarietyDto Variety, Guid? SeedLotId);

public class AddVarietyFromPacketCommandHandler
    : IRequestHandler<AddVarietyFromPacketCommand, AddVarietyFromPacketResult>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public AddVarietyFromPacketCommandHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<AddVarietyFromPacketResult> Handle(
        AddVarietyFromPacketCommand request, CancellationToken ct)
    {
        Guid plantId;

        if (request.ExistingPlantId.HasValue)
        {
            var exists = await _db.Plants.AnyAsync(p => p.Id == request.ExistingPlantId.Value, ct);
            if (!exists) throw new NotFoundException(nameof(Plant), request.ExistingPlantId.Value);
            plantId = request.ExistingPlantId.Value;
        }
        else
        {
            if (string.IsNullOrWhiteSpace(request.NewPlantCommonName))
                throw new DomainException("Either ExistingPlantId or NewPlantCommonName is required.");

            // Check if a global plant with this name already exists to avoid duplicates
            var globalMatch = await _db.Plants
                .FirstOrDefaultAsync(p => p.IsGlobal &&
                    p.CommonName.ToLower() == request.NewPlantCommonName.ToLower(), ct);

            if (globalMatch != null)
            {
                plantId = globalMatch.Id;
            }
            else
            {
                var newPlant = new Plant
                {
                    CommonName = request.NewPlantCommonName,
                    ScientificName = string.Empty,
                    Family = request.NewPlantFamily ?? string.Empty,
                    Category = request.NewPlantCategory ?? PlantCategory.Vegetable,
                    Lifecycle = PlantLifecycle.Annual,
                    SunRequirement = SunRequirement.FullSun,
                    WaterNeeds = "moderate",
                    IsGlobal = false,
                    OwnerId = _currentUser.UserId
                };
                _db.Plants.Add(newPlant);
                plantId = newPlant.Id;
            }
        }

        var variety = new Variety
        {
            PlantId = plantId,
            Name = request.VarietyName,
            DaysToMaturity = request.DaysToMaturity,
            DaysToGerminate = request.DaysToGerminate,
            SowingDepthInches = request.SowingDepthInches,
            SpacingInches = request.SpacingInches,
            DiseaseResistance = request.DiseaseResistance,
            FlavorNotes = request.FlavorNotes,
            ColorNotes = request.ColorNotes,
            Source = request.Supplier,
            Notes = request.Notes,
            IsGlobal = false,
            OwnerId = _currentUser.UserId
        };

        _db.Varieties.Add(variety);

        Guid? seedLotId = null;
        if (request.CreateSeedLot)
        {
            var seedLot = new SeedLot
            {
                OwnerId = _currentUser.UserId,
                VarietyId = variety.Id,
                Supplier = request.Supplier,
                PacketYear = request.PacketYear,
                PacketSeedCount = request.PacketSeedCount,
                PurchaseDate = request.PurchaseDate,
                SeedsRemaining = request.PacketSeedCount
            };
            _db.SeedLots.Add(seedLot);
            seedLotId = seedLot.Id;
        }

        await _db.SaveChangesAsync(ct);

        var plant = await _db.Plants.FirstAsync(p => p.Id == plantId, ct);

        return new AddVarietyFromPacketResult(
            new VarietyDto(
                variety.Id, variety.PlantId, plant.CommonName, variety.Name,
                variety.DaysToMaturity, variety.DaysToGerminate,
                variety.SowingDepthInches, variety.SpacingInches,
                variety.SunRequirement, variety.DiseaseResistance,
                variety.FlavorNotes, variety.ColorNotes, variety.Source,
                variety.Notes, variety.IsGlobal),
            seedLotId);
    }
}
