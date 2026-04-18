using GreenPlot.Application.Common.Interfaces;
using GreenPlot.Application.DTOs;
using GreenPlot.Domain.Entities;
using GreenPlot.Domain.Enums;
using GreenPlot.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GreenPlot.Application.Features.Plantings.Commands;

public record CreatePlantingCommand(
    Guid BedId,
    Guid VarietyId,
    Guid SeasonId,
    Guid? SeedLotId,
    DateOnly StartDate,
    PlantingMethod Method,
    List<Guid> CellIds,
    string? Notes
) : IRequest<PlantingSummaryDto>;

public class CreatePlantingCommandHandler : IRequestHandler<CreatePlantingCommand, PlantingSummaryDto>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public CreatePlantingCommandHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<PlantingSummaryDto> Handle(CreatePlantingCommand request, CancellationToken ct)
    {
        var bed = await _db.Beds
            .Include(b => b.Garden)
            .FirstOrDefaultAsync(b => b.Id == request.BedId, ct)
            ?? throw new NotFoundException(nameof(Domain.Entities.Bed), request.BedId);

        if (bed.Garden.OwnerId != _currentUser.UserId)
            throw new ForbiddenException();

        var variety = await _db.Varieties
            .Include(v => v.Plant)
            .FirstOrDefaultAsync(v => v.Id == request.VarietyId, ct)
            ?? throw new NotFoundException(nameof(Variety), request.VarietyId);

        var planting = new Planting
        {
            OwnerId = _currentUser.UserId,
            BedId = request.BedId,
            VarietyId = request.VarietyId,
            SeasonId = request.SeasonId,
            SeedLotId = request.SeedLotId,
            StartDate = request.StartDate,
            Method = request.Method,
            Notes = request.Notes,
            State = PlantingState.Planned
        };

        _db.Plantings.Add(planting);

        foreach (var cellId in request.CellIds)
        {
            _db.PlantingCells.Add(new PlantingCell
            {
                PlantingId = planting.Id,
                BedCellId = cellId
            });
        }

        await _db.SaveChangesAsync(ct);

        return new PlantingSummaryDto(
            planting.Id, planting.VarietyId, variety.Name,
            variety.Plant.CommonName, variety.Plant.Family,
            planting.State, planting.Method, planting.StartDate);
    }
}
