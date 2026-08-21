using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SeatSync.Application.Abstractions;
using SeatSync.Domain.Enums;
using SeatSync.Infrastructure.Persistence;
namespace SeatSync.Infrastructure.Background;
public sealed class HoldExpiryService(IServiceScopeFactory scopes, ILogger<HoldExpiryService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try { await ExpireAsync(stoppingToken); } catch (Exception ex) { logger.LogError(ex, "Failed to expire seat holds."); }
            await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
        }
    }
    private async Task ExpireAsync(CancellationToken ct)
    {
        using var scope = scopes.CreateScope(); var db = scope.ServiceProvider.GetRequiredService<AppDbContext>(); var cache = scope.ServiceProvider.GetRequiredService<ISeatAvailabilityCache>();
        var expired = await db.SeatHolds.Include(x => x.Items).ThenInclude(x => x.Seat).Where(x => x.Status == HoldStatus.Active && x.ExpiresAtUtc <= DateTime.UtcNow).ToListAsync(ct);
        foreach (var hold in expired) { hold.Status = HoldStatus.Expired; foreach (var item in hold.Items) item.Seat.ReleaseHold(hold.Id); }
        if (expired.Count == 0) return; await db.SaveChangesAsync(ct); foreach (var eventId in expired.Select(x => x.EventId).Distinct()) await cache.InvalidateAsync(eventId);
    }
}
