using SeatSync.Domain.Enums;

namespace SeatSync.Domain.Entities;

public sealed class Seat
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid EventId { get; set; }
    public Event Event { get; set; } = null!;
    public string Section { get; set; } = string.Empty;
    public string Row { get; set; } = string.Empty;
    public int Number { get; set; }
    public decimal Price { get; set; }
    public SeatStatus Status { get; private set; } = SeatStatus.Available;
    public Guid? ActiveHoldId { get; private set; }
    public DateTime? HoldExpiresAtUtc { get; private set; }
    public byte[] RowVersion { get; set; } = Array.Empty<byte>();

    public bool CanBeHeld(DateTime nowUtc) =>
        Status == SeatStatus.Available ||
        (Status == SeatStatus.Held && HoldExpiresAtUtc <= nowUtc);

    public void PlaceHold(Guid holdId, DateTime expiresAtUtc, DateTime nowUtc)
    {
        if (!CanBeHeld(nowUtc)) throw new InvalidOperationException("Seat is not available.");
        Status = SeatStatus.Held;
        ActiveHoldId = holdId;
        HoldExpiresAtUtc = expiresAtUtc;
    }

    public void Confirm(Guid holdId)
    {
        if (Status != SeatStatus.Held || ActiveHoldId != holdId)
            throw new InvalidOperationException("Seat does not belong to this hold.");
        Status = SeatStatus.Reserved;
        ActiveHoldId = null;
        HoldExpiresAtUtc = null;
    }

    public void ReleaseHold(Guid holdId)
    {
        if (Status != SeatStatus.Held || ActiveHoldId != holdId) return;
        Status = SeatStatus.Available;
        ActiveHoldId = null;
        HoldExpiresAtUtc = null;
    }

    public void ReleaseReservation()
    {
        if (Status != SeatStatus.Reserved) return;
        Status = SeatStatus.Available;
    }
}
