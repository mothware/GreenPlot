using GreenPlot.Application.Common.Interfaces;
using GreenPlot.Application.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GreenPlot.Application.Features.Calendar.Queries;

public record GetCalendarEventsQuery(
    DateTime From,
    DateTime To,
    Guid? GardenId,
    Guid? BedId
) : IRequest<List<CalendarEventDto>>;

public class GetCalendarEventsQueryHandler : IRequestHandler<GetCalendarEventsQuery, List<CalendarEventDto>>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public GetCalendarEventsQueryHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<List<CalendarEventDto>> Handle(GetCalendarEventsQuery request, CancellationToken ct)
    {
        var activitiesQuery = _db.Activities
            .Include(a => a.Planting)
                .ThenInclude(p => p.Variety)
                    .ThenInclude(v => v.Plant)
            .Include(a => a.Planting)
                .ThenInclude(p => p.Bed)
                    .ThenInclude(b => b.Garden)
            .Where(a =>
                a.Planting.OwnerId == _currentUser.UserId &&
                a.OccurredAt >= request.From &&
                a.OccurredAt <= request.To);

        if (request.GardenId.HasValue)
            activitiesQuery = activitiesQuery.Where(a => a.Planting.Bed.GardenId == request.GardenId.Value);

        if (request.BedId.HasValue)
            activitiesQuery = activitiesQuery.Where(a => a.Planting.BedId == request.BedId.Value);

        var activities = await activitiesQuery.ToListAsync(ct);

        return activities.Select(a => new CalendarEventDto(
            a.Id,
            $"{a.Type}: {a.Planting.Variety.Name}",
            a.OccurredAt,
            null,
            a.Type.ToString(),
            a.PlantingId,
            a.Planting.Bed.GardenId,
            a.Planting.BedId,
            a.Planting.Variety.Name,
            a.Planting.Variety.Plant.CommonName,
            a.Planting.Bed.Garden.Name,
            a.Planting.Bed.Name
        )).ToList();
    }
}
