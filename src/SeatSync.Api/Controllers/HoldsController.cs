using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SeatSync.Application.Abstractions;
using SeatSync.Application.Contracts.Holds;
using SeatSync.Application.Contracts.Reservations;
namespace SeatSync.Api.Controllers;
[Authorize, ApiController, Route("api")]
public sealed class HoldsController(IReservationService reservations) : ControllerBase
{
    [HttpPost("events/{eventId:guid}/holds")]
    public async Task<ActionResult<HoldResponse>> Create(Guid eventId, CreateHoldRequest request, [FromHeader(Name = "Idempotency-Key")] string key, CancellationToken ct)
    { var result = await reservations.CreateHoldAsync(eventId, request, key, ct); return Created($"/api/holds/{result.Id}", result); }
    [HttpPost("holds/{holdId:guid}/confirm")]
    public async Task<ActionResult<ReservationResponse>> Confirm(Guid holdId, CancellationToken ct)
    { var result = await reservations.ConfirmHoldAsync(holdId, ct); return Created($"/api/reservations/{result.Id}", result); }
}
