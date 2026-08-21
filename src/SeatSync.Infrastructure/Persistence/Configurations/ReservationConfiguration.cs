using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SeatSync.Domain.Entities;
namespace SeatSync.Infrastructure.Persistence.Configurations;
public sealed class ReservationConfiguration : IEntityTypeConfiguration<Reservation>
{
    public void Configure(EntityTypeBuilder<Reservation> b) { b.HasMany(x => x.Seats).WithOne(x => x.Reservation).HasForeignKey(x => x.ReservationId); b.HasIndex(x => x.UserId); }
}
public sealed class ReservationSeatConfiguration : IEntityTypeConfiguration<ReservationSeat>
{
    public void Configure(EntityTypeBuilder<ReservationSeat> b) { b.HasKey(x => new { x.ReservationId, x.SeatId }); b.Property(x => x.PriceAtBooking).HasPrecision(10, 2); b.HasOne(x => x.Seat).WithMany().HasForeignKey(x => x.SeatId).OnDelete(DeleteBehavior.Restrict); }
}
