using GreenPlot.Application.Common.Interfaces;
using GreenPlot.Application.DTOs;
using GreenPlot.Domain.Entities;
using GreenPlot.Domain.Enums;
using MediatR;

namespace GreenPlot.Application.Features.Plants.Commands;

public record CreatePlantCommand(
    string CommonName,
    string ScientificName,
    string Family,
    PlantCategory Category,
    PlantLifecycle Lifecycle,
    SunRequirement SunRequirement,
    string WaterNeeds,
    int? DaysToGerminateMin,
    int? DaysToGerminateMax,
    int? DaysToMaturityMin,
    int? DaysToMaturityMax,
    decimal? SowingDepthInches,
    decimal? SpacingInches,
    string? Notes
) : IRequest<PlantDto>;

public class CreatePlantCommandHandler : IRequestHandler<CreatePlantCommand, PlantDto>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public CreatePlantCommandHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<PlantDto> Handle(CreatePlantCommand request, CancellationToken ct)
    {
        var plant = new Plant
        {
            CommonName = request.CommonName,
            ScientificName = request.ScientificName,
            Family = request.Family,
            Category = request.Category,
            Lifecycle = request.Lifecycle,
            SunRequirement = request.SunRequirement,
            WaterNeeds = request.WaterNeeds,
            DaysToGerminateMin = request.DaysToGerminateMin,
            DaysToGerminateMax = request.DaysToGerminateMax,
            DaysToMaturityMin = request.DaysToMaturityMin,
            DaysToMaturityMax = request.DaysToMaturityMax,
            SowingDepthInches = request.SowingDepthInches,
            SpacingInches = request.SpacingInches,
            Notes = request.Notes,
            IsGlobal = false,
            OwnerId = _currentUser.UserId
        };

        _db.Plants.Add(plant);
        await _db.SaveChangesAsync(ct);

        return new PlantDto(
            plant.Id, plant.CommonName, plant.ScientificName, plant.Family,
            plant.Category, plant.Lifecycle, plant.SunRequirement, plant.WaterNeeds,
            plant.DaysToGerminateMin, plant.DaysToGerminateMax,
            plant.DaysToMaturityMin, plant.DaysToMaturityMax,
            plant.SowingDepthInches, plant.SpacingInches,
            plant.HardinessZoneMin, plant.HardinessZoneMax,
            plant.Notes, plant.IsGlobal, 0);
    }
}
