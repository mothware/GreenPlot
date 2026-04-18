using ThePatch.Application.Features.Calendar.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ThePatch.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/calendar")]
public class CalendarController : ControllerBase
{
    private readonly IMediator _mediator;

    public CalendarController(IMediator mediator) => _mediator = mediator;

    [HttpGet("events")]
    public async Task<IActionResult> GetEvents(
        [FromQuery] DateTime from,
        [FromQuery] DateTime to,
        [FromQuery] Guid? gardenId,
        [FromQuery] Guid? bedId,
        CancellationToken ct)
    {
        var events = await _mediator.Send(new GetCalendarEventsQuery(from, to, gardenId, bedId), ct);
        return Ok(events);
    }
}
