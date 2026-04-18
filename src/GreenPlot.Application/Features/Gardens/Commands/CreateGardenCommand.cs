using GreenPlot.Application.Common.Interfaces;
using GreenPlot.Application.DTOs;
using GreenPlot.Domain.Entities;
using MediatR;

namespace GreenPlot.Application.Features.Gardens.Commands;

public record CreateGardenCommand(
    string Name,
    string? ZipCode,
    string? Notes
) : IRequest<GardenDto>;

public class CreateGardenCommandHandler : IRequestHandler<CreateGardenCommand, GardenDto>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;
    private readonly IFrostDateService _frostDateService;

    public CreateGardenCommandHandler(
        IApplicationDbContext db,
        ICurrentUserService currentUser,
        IFrostDateService frostDateService)
    {
        _db = db;
        _currentUser = currentUser;
        _frostDateService = frostDateService;
    }

    public async Task<GardenDto> Handle(CreateGardenCommand request, CancellationToken ct)
    {
        var garden = new Garden
        {
            OwnerId = _currentUser.UserId,
            Name = request.Name,
            ZipCode = request.ZipCode,
            Notes = request.Notes
        };

        if (!string.IsNullOrWhiteSpace(request.ZipCode))
        {
            var frostDates = await _frostDateService.GetFrostDatesAsync(request.ZipCode, ct);
            if (frostDates != null)
            {
                garden.LastFrostDate = frostDates.LastSpringFrost;
                garden.FirstFrostDate = frostDates.FirstFallFrost;
                garden.HardinessZone = frostDates.HardinessZone;
            }
        }

        _db.Gardens.Add(garden);
        await _db.SaveChangesAsync(ct);

        return new GardenDto(
            garden.Id, garden.Name, garden.ZipCode, garden.HardinessZone,
            garden.LastFrostDate, garden.FirstFrostDate, garden.Notes,
            garden.IsArchived, 0, garden.CreatedAt);
    }
}
