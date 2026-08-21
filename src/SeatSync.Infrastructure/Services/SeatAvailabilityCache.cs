using System.Text.Json;
using SeatSync.Application.Abstractions;
using SeatSync.Application.Contracts.Events;
using StackExchange.Redis;
namespace SeatSync.Infrastructure.Services;
public sealed class SeatAvailabilityCache(IConnectionMultiplexer redis) : ISeatAvailabilityCache
{
    private IDatabase Db => redis.GetDatabase();
    private static string Key(Guid eventId) => $"seatsync:event:{eventId}:seats";
    public async Task<IReadOnlyList<SeatResponse>?> GetAsync(Guid eventId)
    {
        var value = await Db.StringGetAsync(Key(eventId));
        return value.IsNullOrEmpty ? null : JsonSerializer.Deserialize<List<SeatResponse>>(value.ToString());
    }
    public Task SetAsync(Guid eventId, IReadOnlyList<SeatResponse> seats) => Db.StringSetAsync(Key(eventId), JsonSerializer.Serialize(seats), TimeSpan.FromSeconds(15));
    public Task InvalidateAsync(Guid eventId) => Db.KeyDeleteAsync(Key(eventId));
}
