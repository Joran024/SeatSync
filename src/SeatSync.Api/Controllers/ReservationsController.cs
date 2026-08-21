using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SeatSync.Application.Abstractions;
using SeatSync.Application.Contracts.Reservations;
namespace SeatSync.Api.Controllers;
[Authorize, ApiController, Route("api/reservations")]
public sealed class ReservationsController(IReservationService reservations) : ControllerBase
{
    [HttpGet("me")] public Task<IReadOnlyList<ReservationResponse>> Mine(CancellationToken ct) => reservations.GetMineAsync(ct);
    [HttpDelete("{reservationId:guid}")] public async Task<IActionResult> Cancel(Guid reservationId, CancellationToken ct) { await reservations.CancelAsync(reservationId, ct); return NoContent(); }
}
