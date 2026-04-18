using GreenPlot.Application.Common.Interfaces;
using GreenPlot.Application.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GreenPlot.Application.Features.Gardens.Queries;

public record GetGardensQuery : IRequest<List<GardenDto>>;

public class GetGardensQueryHandler : IRequestHandler<GetGardensQuery, List<GardenDto>>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public GetGardensQueryHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<List<GardenDto>> Handle(GetGardensQuery request, CancellationToken ct)
    {
        return await _db.Gardens
            .Where(g => g.OwnerId == _currentUser.UserId)
            .Select(g => new GardenDto(
                g.Id,
                g.Name,
                g.ZipCode,
                g.HardinessZone,
                g.LastFrostDate,
                g.FirstFrostDate,
                g.Notes,
                g.IsArchived,
                g.Beds.Count,
                g.CreatedAt))
            .ToListAsync(ct);
    }
}
