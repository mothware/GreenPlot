using GreenPlot.Application.Common.Interfaces;
using GreenPlot.Application.DTOs;
using GreenPlot.Domain.Entities;
using GreenPlot.Domain.Enums;
using GreenPlot.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GreenPlot.Application.Features.Activities.Commands;

public record LogActivityCommand(
    Guid PlantingId,
    ActivityType Type,
    DateTime OccurredAt,
    int? Quantity,
    decimal? WeightGrams,
    string? Notes,
    List<string>? PhotoUrls
) : IRequest<ActivityDto>;

public class LogActivityCommandHandler : IRequestHandler<LogActivityCommand, ActivityDto>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public LogActivityCommandHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<ActivityDto> Handle(LogActivityCommand request, CancellationToken ct)
    {
        var planting = await _db.Plantings
            .FirstOrDefaultAsync(p => p.Id == request.PlantingId && p.OwnerId == _currentUser.UserId, ct)
            ?? throw new NotFoundException(nameof(Planting), request.PlantingId);

        var activity = new Activity
        {
            PlantingId = request.PlantingId,
            Type = request.Type,
            OccurredAt = request.OccurredAt,
            Quantity = request.Quantity,
            WeightGrams = request.WeightGrams,
            Notes = request.Notes,
            PhotoUrls = request.PhotoUrls ?? []
        };

        _db.Activities.Add(activity);

        // Advance planting state based on activity type
        planting.State = request.Type switch
        {
            ActivityType.Sown => PlantingState.Sown,
            ActivityType.Germinated => PlantingState.Germinated,
            ActivityType.PottedUp => PlantingState.PottedUp,
            ActivityType.HardenedOff => PlantingState.HardenedOff,
            ActivityType.Transplanted => PlantingState.Transplanted,
            ActivityType.Flowering => PlantingState.Flowering,
            ActivityType.Harvested => PlantingState.Harvested,
            ActivityType.Ended => PlantingState.Ended,
            _ => planting.State
        };

        await _db.SaveChangesAsync(ct);

        return new ActivityDto(
            activity.Id, activity.PlantingId, activity.Type,
            activity.OccurredAt, activity.Quantity, activity.WeightGrams,
            activity.Notes, activity.PhotoUrls);
    }
}
