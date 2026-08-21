using SeatSync.Application.Contracts.Events;
namespace SeatSync.Application.Abstractions;
public interface ISeatAvailabilityCache
{
    Task<IReadOnlyList<SeatResponse>?> GetAsync(Guid eventId);
    Task SetAsync(Guid eventId, IReadOnlyList<SeatResponse> seats);
    Task InvalidateAsync(Guid eventId);
}
