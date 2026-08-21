namespace SeatSync.Domain.Entities;
public sealed class Event
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string VenueName { get; set; } = string.Empty;
    public DateTime StartsAtUtc { get; set; }
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public ICollection<Seat> Seats { get; set; } = new List<Seat>();
}
