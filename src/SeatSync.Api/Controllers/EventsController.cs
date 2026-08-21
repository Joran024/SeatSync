using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SeatSync.Application.Abstractions;
using SeatSync.Application.Contracts.Events;
namespace SeatSync.Api.Controllers;
[ApiController, Route("api/events")]
public sealed class EventsController(IEventService events) : ControllerBase
{
    [AllowAnonymous, HttpGet] public Task<IReadOnlyList<EventSummaryResponse>> GetAll(CancellationToken ct) => events.GetEventsAsync(ct);
    [AllowAnonymous, HttpGet("{eventId:guid}/seats")] public Task<IReadOnlyList<SeatResponse>> GetSeats(Guid eventId, CancellationToken ct) => events.GetSeatsAsync(eventId, ct);
}
