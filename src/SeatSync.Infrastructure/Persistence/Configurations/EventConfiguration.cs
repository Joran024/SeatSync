using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SeatSync.Domain.Entities;
namespace SeatSync.Infrastructure.Persistence.Configurations;
public sealed class EventConfiguration : IEntityTypeConfiguration<Event>
{
    public void Configure(EntityTypeBuilder<Event> b) { b.Property(x => x.Name).HasMaxLength(160).IsRequired(); b.Property(x => x.VenueName).HasMaxLength(160).IsRequired(); }
}
