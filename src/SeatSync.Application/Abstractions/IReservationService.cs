using SeatSync.Application.Contracts.Holds;
using SeatSync.Application.Contracts.Reservations;
namespace SeatSync.Application.Abstractions;
public interface IReservationService
{
    Task<HoldResponse> CreateHoldAsync(Guid eventId, CreateHoldRequest request, string idempotencyKey, CancellationToken ct);
    Task<ReservationResponse> ConfirmHoldAsync(Guid holdId, CancellationToken ct);
    Task<IReadOnlyList<ReservationResponse>> GetMineAsync(CancellationToken ct);
    Task CancelAsync(Guid reservationId, CancellationToken ct);
}
