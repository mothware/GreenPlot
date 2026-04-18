using ThePatch.Application.Common.Interfaces;
using ThePatch.Application.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ThePatch.Application.Features.Varieties.Queries;

public record GetVarietiesByPlantQuery(Guid PlantId) : IRequest<List<VarietyDto>>;

public class GetVarietiesByPlantQueryHandler : IRequestHandler<GetVarietiesByPlantQuery, List<VarietyDto>>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public GetVarietiesByPlantQueryHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<List<VarietyDto>> Handle(GetVarietiesByPlantQuery request, CancellationToken ct)
    {
        return await _db.Varieties
            .Include(v => v.Plant)
            .Where(v => v.PlantId == request.PlantId &&
                        (v.IsGlobal || v.OwnerId == _currentUser.UserId))
            .Select(v => new VarietyDto(
                v.Id, v.PlantId, v.Plant.CommonName, v.Name,
                v.DaysToMaturity, v.DaysToGerminate,
                v.SowingDepthInches, v.SpacingInches,
                v.SunRequirement, v.DiseaseResistance,
                v.FlavorNotes, v.ColorNotes, v.Source, v.Notes, v.IsGlobal))
            .ToListAsync(ct);
    }
}
