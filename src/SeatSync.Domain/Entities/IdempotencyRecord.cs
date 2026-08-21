namespace SeatSync.Domain.Entities;
public sealed class IdempotencyRecord
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public string Key { get; set; } = string.Empty;
    public Guid HoldId { get; set; }
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}
