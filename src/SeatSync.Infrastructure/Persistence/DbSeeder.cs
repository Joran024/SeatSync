using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SeatSync.Domain.Entities;
namespace SeatSync.Infrastructure.Persistence;
public static class DbSeeder
{
    public static async Task SeedSeatSyncAsync(this IServiceProvider services)
    {
        using var scope = services.CreateScope(); var db = scope.ServiceProvider.GetRequiredService<AppDbContext>(); await db.Database.EnsureCreatedAsync();
        if (await db.Events.AnyAsync()) return;
        var e = new Event { Name = "Neon Arena Live", VenueName = "Brussels Expo", StartsAtUtc = DateTime.UtcNow.Date.AddDays(30).AddHours(18) };
        foreach (var row in new[] { "A", "B", "C", "D" }) for (var number = 1; number <= 10; number++) e.Seats.Add(new Seat { Section = "Floor", Row = row, Number = number, Price = row is "A" or "B" ? 89m : 69m });
        db.Events.Add(e); await db.SaveChangesAsync();
    }
}
