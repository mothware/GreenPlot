using ThePatch.Application.Common.Interfaces;
using ThePatch.Application.DTOs;
using ThePatch.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ThePatch.Application.Features.Beds.Queries;

public record GetBedDetailQuery(Guid BedId) : IRequest<BedDetailDto>;

public class GetBedDetailQueryHandler : IRequestHandler<GetBedDetailQuery, BedDetailDto>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public GetBedDetailQueryHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<BedDetailDto> Handle(GetBedDetailQuery request, CancellationToken ct)
    {
        var bed = await _db.Beds
            .Include(b => b.Cells)
                .ThenInclude(c => c.PlantingCells)
                    .ThenInclude(pc => pc.Planting)
                        .ThenInclude(p => p.Variety)
                            .ThenInclude(v => v.Plant)
            .Include(b => b.Plantings)
                .ThenInclude(p => p.Variety)
                    .ThenInclude(v => v.Plant)
            .FirstOrDefaultAsync(b => b.Id == request.BedId, ct)
            ?? throw new NotFoundException(nameof(Domain.Entities.Bed), request.BedId);

        if (bed.Garden.OwnerId != _currentUser.UserId)
            throw new ForbiddenException();

        var cells = bed.Cells.Select(c =>
        {
            var pc = c.PlantingCells.FirstOrDefault();
            PlantingSummaryDto? planting = pc?.Planting == null ? null : new PlantingSummaryDto(
                pc.Planting.Id,
                pc.Planting.VarietyId,
                pc.Planting.Variety.Name,
                pc.Planting.Variety.Plant.CommonName,
                pc.Planting.Variety.Plant.Family,
                pc.Planting.State,
                pc.Planting.Method,
                pc.Planting.StartDate);

            return new BedCellDto(c.Id, c.X, c.Y, planting);
        }).ToList();

        var plantings = bed.Plantings.Select(p => new PlantingSummaryDto(
            p.Id, p.VarietyId, p.Variety.Name,
            p.Variety.Plant.CommonName, p.Variety.Plant.Family,
            p.State, p.Method, p.StartDate)).ToList();

        return new BedDetailDto(
            bed.Id, bed.GardenId, bed.Name, bed.Type, bed.Shape,
            bed.WidthFeet, bed.LengthFeet, bed.GridCellSizeFeet,
            bed.SunExposure, bed.SoilNotes, bed.IsArchived, cells, plantings);
    }
}
