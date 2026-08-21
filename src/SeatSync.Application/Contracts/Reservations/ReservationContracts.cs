namespace SeatSync.Application.Contracts.Reservations;
public sealed record ReservedSeatResponse(Guid SeatId, string Section, string Row, int Number, decimal Price);
public sealed record ReservationResponse(Guid Id, Guid EventId, string Status, DateTime CreatedAtUtc, IReadOnlyList<ReservedSeatResponse> Seats);
