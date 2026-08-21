using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SeatSync.Domain.Entities;
namespace SeatSync.Infrastructure.Persistence.Configurations;
public sealed class HoldConfiguration : IEntityTypeConfiguration<SeatHold>
{
    public void Configure(EntityTypeBuilder<SeatHold> b) { b.HasMany(x => x.Items).WithOne(x => x.Hold).HasForeignKey(x => x.HoldId); b.HasIndex(x => new { x.Status, x.ExpiresAtUtc }); }
}
public sealed class HoldItemConfiguration : IEntityTypeConfiguration<SeatHoldItem>
{
    public void Configure(EntityTypeBuilder<SeatHoldItem> b) { b.HasKey(x => new { x.HoldId, x.SeatId }); b.HasOne(x => x.Seat).WithMany().HasForeignKey(x => x.SeatId).OnDelete(DeleteBehavior.Restrict); }
}
