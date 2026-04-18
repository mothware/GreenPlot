using ThePatch.Application.Common.Interfaces;
using ThePatch.Application.DTOs;
using ThePatch.Domain.Entities;
using ThePatch.Domain.Enums;
using ThePatch.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ThePatch.Application.Features.Beds.Commands;

public record CreateBedCommand(
    Guid GardenId,
    string Name,
    BedType Type,
    BedShape Shape,
    decimal WidthFeet,
    decimal LengthFeet,
    decimal GridCellSizeFeet,
    SunRequirement? SunExposure,
    string? SoilNotes
) : IRequest<BedSummaryDto>;

public class CreateBedCommandHandler : IRequestHandler<CreateBedCommand, BedSummaryDto>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public CreateBedCommandHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<BedSummaryDto> Handle(CreateBedCommand request, CancellationToken ct)
    {
        var garden = await _db.Gardens.FirstOrDefaultAsync(g => g.Id == request.GardenId, ct)
            ?? throw new NotFoundException(nameof(Domain.Entities.Garden), request.GardenId);

        if (garden.OwnerId != _currentUser.UserId)
            throw new ForbiddenException();

        var bed = new Bed
        {
            GardenId = request.GardenId,
            Name = request.Name,
            Type = request.Type,
            Shape = request.Shape,
            WidthFeet = request.WidthFeet,
            LengthFeet = request.LengthFeet,
            GridCellSizeFeet = request.GridCellSizeFeet,
            SunExposure = request.SunExposure,
            SoilNotes = request.SoilNotes
        };

        _db.Beds.Add(bed);

        // Generate grid cells for rectangle/square beds
        if (request.Shape is BedShape.Rectangle or BedShape.Square)
        {
            var cols = (int)Math.Ceiling(request.WidthFeet / request.GridCellSizeFeet);
            var rows = (int)Math.Ceiling(request.LengthFeet / request.GridCellSizeFeet);
            for (var r = 0; r < rows; r++)
            for (var c = 0; c < cols; c++)
                _db.BedCells.Add(new BedCell { BedId = bed.Id, X = c, Y = r });
        }

        await _db.SaveChangesAsync(ct);

        return new BedSummaryDto(
            bed.Id, bed.Name, bed.Type, bed.Shape,
            bed.WidthFeet, bed.LengthFeet, bed.GridCellSizeFeet,
            bed.SunExposure, bed.IsArchived, 0);
    }
}
