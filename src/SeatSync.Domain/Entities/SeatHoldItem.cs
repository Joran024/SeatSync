namespace SeatSync.Domain.Entities;
public sealed class SeatHoldItem
{
    public Guid HoldId { get; set; }
    public SeatHold Hold { get; set; } = null!;
    public Guid SeatId { get; set; }
    public Seat Seat { get; set; } = null!;
}
