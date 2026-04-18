using GreenPlot.Application.Common.Interfaces;
using GreenPlot.Application.DTOs;
using GreenPlot.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GreenPlot.Application.Features.Plants.Queries;

public record GetPlantByIdQuery(Guid PlantId) : IRequest<PlantDto>;

public class GetPlantByIdQueryHandler : IRequestHandler<GetPlantByIdQuery, PlantDto>
{
    private readonly IApplicationDbContext _db;

    public GetPlantByIdQueryHandler(IApplicationDbContext db) => _db = db;

    public async Task<PlantDto> Handle(GetPlantByIdQuery request, CancellationToken ct)
    {
        var plant = await _db.Plants
            .Include(p => p.Varieties)
            .FirstOrDefaultAsync(p => p.Id == request.PlantId, ct)
            ?? throw new NotFoundException(nameof(Domain.Entities.Plant), request.PlantId);

        return new PlantDto(
            plant.Id,
            plant.CommonName,
            plant.ScientificName,
            plant.Family,
            plant.Category,
            plant.Lifecycle,
            plant.SunRequirement,
            plant.WaterNeeds,
            plant.DaysToGerminateMin,
            plant.DaysToGerminateMax,
            plant.DaysToMaturityMin,
            plant.DaysToMaturityMax,
            plant.SowingDepthInches,
            plant.SpacingInches,
            plant.HardinessZoneMin,
            plant.HardinessZoneMax,
            plant.Notes,
            plant.IsGlobal,
            plant.Varieties.Count);
    }
}
