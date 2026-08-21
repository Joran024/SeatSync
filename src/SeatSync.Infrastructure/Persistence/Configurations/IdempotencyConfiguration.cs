using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SeatSync.Domain.Entities;
namespace SeatSync.Infrastructure.Persistence.Configurations;
public sealed class IdempotencyConfiguration : IEntityTypeConfiguration<IdempotencyRecord>
{
    public void Configure(EntityTypeBuilder<IdempotencyRecord> b) { b.Property(x => x.Key).HasMaxLength(120).IsRequired(); b.HasIndex(x => new { x.UserId, x.Key }).IsUnique(); }
}
