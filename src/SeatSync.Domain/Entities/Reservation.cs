using SeatSync.Domain.Enums;

namespace SeatSync.Domain.Entities;

public sealed class Reservation
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid EventId { get; set; }
    public Guid UserId { get; set; }
    public ReservationStatus Status { get; set; } = ReservationStatus.Confirmed;
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? CancelledAtUtc { get; set; }
    public ICollection<ReservationSeat> Seats { get; set; } = new List<ReservationSeat>();
}
