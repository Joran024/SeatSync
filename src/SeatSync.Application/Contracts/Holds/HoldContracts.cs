namespace SeatSync.Application.Contracts.Holds;
public sealed record CreateHoldRequest(IReadOnlyList<Guid> SeatIds);
public sealed record HoldResponse(Guid Id, Guid EventId, DateTime ExpiresAtUtc, string Status, IReadOnlyList<Guid> SeatIds);
