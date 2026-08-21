using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SeatSync.Domain.Entities;
namespace SeatSync.Infrastructure.Persistence.Configurations;
public sealed class SeatConfiguration : IEntityTypeConfiguration<Seat>
{
    public void Configure(EntityTypeBuilder<Seat> b)
    {
        b.Property(x => x.Section).HasMaxLength(40).IsRequired(); b.Property(x => x.Row).HasMaxLength(12).IsRequired();
        b.Property(x => x.Price).HasPrecision(10, 2); b.Property(x => x.RowVersion).IsRowVersion();
        b.HasIndex(x => new { x.EventId, x.Section, x.Row, x.Number }).IsUnique();
    }
}
