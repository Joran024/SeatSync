using SeatSync.Application.Contracts.Events;
namespace SeatSync.Application.Abstractions;
public interface IEventService
{
    Task<IReadOnlyList<EventSummaryResponse>> GetEventsAsync(CancellationToken ct);
    Task<IReadOnlyList<SeatResponse>> GetSeatsAsync(Guid eventId, CancellationToken ct);
}
