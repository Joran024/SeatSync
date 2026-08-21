using SeatSync.Domain.Enums;

namespace SeatSync.Domain.Entities;

public sealed class SeatHold
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid EventId { get; set; }
    public Guid UserId { get; set; }
    public HoldStatus Status { get; set; } = HoldStatus.Active;
    public DateTime ExpiresAtUtc { get; set; }
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public ICollection<SeatHoldItem> Items { get; set; } = new List<SeatHoldItem>();
    public bool IsExpired(DateTime nowUtc) => Status == HoldStatus.Active && ExpiresAtUtc <= nowUtc;
}
