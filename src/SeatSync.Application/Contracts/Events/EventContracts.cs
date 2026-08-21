namespace SeatSync.Application.Contracts.Events;
public sealed record EventSummaryResponse(Guid Id, string Name, string VenueName, DateTime StartsAtUtc);
public sealed record SeatResponse(Guid Id, string Section, string Row, int Number, decimal Price, string Status, DateTime? HoldExpiresAtUtc);
