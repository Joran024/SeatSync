namespace SeatSync.Domain.Entities;
public sealed class ReservationSeat
{
    public Guid ReservationId { get; set; }
    public Reservation Reservation { get; set; } = null!;
    public Guid SeatId { get; set; }
    public Seat Seat { get; set; } = null!;
    public decimal PriceAtBooking { get; set; }
}
