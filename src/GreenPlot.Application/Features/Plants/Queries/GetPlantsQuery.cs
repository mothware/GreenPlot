using GreenPlot.Application.Common.Interfaces;
using GreenPlot.Application.DTOs;
using GreenPlot.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GreenPlot.Application.Features.Plants.Queries;

public record GetPlantsQuery(
    string? Search,
    PlantCategory? Category,
    PlantLifecycle? Lifecycle,
    SunRequirement? SunRequirement,
    int? DaysToMaturityMin,
    int? DaysToMaturityMax,
    int Page = 1,
    int PageSize = 25
) : IRequest<PagedResult<PlantDto>>;

public record PagedResult<T>(List<T> Items, int TotalCount, int Page, int PageSize);

public class GetPlantsQueryHandler : IRequestHandler<GetPlantsQuery, PagedResult<PlantDto>>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public GetPlantsQueryHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<PagedResult<PlantDto>> Handle(GetPlantsQuery request, CancellationToken ct)
    {
        var query = _db.Plants
            .Where(p => p.IsGlobal || p.OwnerId == _currentUser.UserId);

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var s = request.Search.ToLower();
            query = query.Where(p =>
                p.CommonName.ToLower().Contains(s) ||
                p.ScientificName.ToLower().Contains(s));
        }

        if (request.Category.HasValue)
            query = query.Where(p => p.Category == request.Category.Value);

        if (request.Lifecycle.HasValue)
            query = query.Where(p => p.Lifecycle == request.Lifecycle.Value);

        if (request.SunRequirement.HasValue)
            query = query.Where(p => p.SunRequirement == request.SunRequirement.Value);

        if (request.DaysToMaturityMin.HasValue)
            query = query.Where(p => p.DaysToMaturityMin >= request.DaysToMaturityMin.Value);

        if (request.DaysToMaturityMax.HasValue)
            query = query.Where(p => p.DaysToMaturityMax <= request.DaysToMaturityMax.Value);

        var total = await query.CountAsync(ct);

        var plants = await query
            .OrderBy(p => p.CommonName)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(p => new PlantDto(
                p.Id,
                p.CommonName,
                p.ScientificName,
                p.Family,
                p.Category,
                p.Lifecycle,
                p.SunRequirement,
                p.WaterNeeds,
                p.DaysToGerminateMin,
                p.DaysToGerminateMax,
                p.DaysToMaturityMin,
                p.DaysToMaturityMax,
                p.SowingDepthInches,
                p.SpacingInches,
                p.HardinessZoneMin,
                p.HardinessZoneMax,
                p.Notes,
                p.IsGlobal,
                p.Varieties.Count))
            .ToListAsync(ct);

        return new PagedResult<PlantDto>(plants, total, request.Page, request.PageSize);
    }
}
