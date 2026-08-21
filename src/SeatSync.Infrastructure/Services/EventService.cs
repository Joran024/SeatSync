using Microsoft.EntityFrameworkCore;
using SeatSync.Application.Abstractions;
using SeatSync.Application.Contracts.Events;
using SeatSync.Application.Exceptions;
using SeatSync.Domain.Enums;
using SeatSync.Infrastructure.Persistence;
namespace SeatSync.Infrastructure.Services;
public sealed class EventService(AppDbContext db, ISeatAvailabilityCache cache) : IEventService
{
    public async Task<IReadOnlyList<EventSummaryResponse>> GetEventsAsync(CancellationToken ct) => await db.Events.AsNoTracking().OrderBy(x => x.StartsAtUtc)
        .Select(x => new EventSummaryResponse(x.Id, x.Name, x.VenueName, x.StartsAtUtc)).ToListAsync(ct);
    public async Task<IReadOnlyList<SeatResponse>> GetSeatsAsync(Guid eventId, CancellationToken ct)
    {
        var cached = await cache.GetAsync(eventId); if (cached is not null) return cached;
        if (!await db.Events.AnyAsync(x => x.Id == eventId, ct)) throw new NotFoundException("Event not found.");
        var now = DateTime.UtcNow;
        var seats = await db.Seats.AsNoTracking().Where(x => x.EventId == eventId).OrderBy(x => x.Section).ThenBy(x => x.Row).ThenBy(x => x.Number).ToListAsync(ct);
        var result = seats.Select(x => new SeatResponse(x.Id, x.Section, x.Row, x.Number, x.Price,
            x.Status == SeatStatus.Held && x.HoldExpiresAtUtc <= now ? SeatStatus.Available.ToString() : x.Status.ToString(), x.HoldExpiresAtUtc)).ToList();
        await cache.SetAsync(eventId, result); return result;
    }
}
