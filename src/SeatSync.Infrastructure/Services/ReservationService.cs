using System.Data;
using Microsoft.EntityFrameworkCore;
using SeatSync.Application.Abstractions;
using SeatSync.Application.Contracts.Holds;
using SeatSync.Application.Contracts.Reservations;
using SeatSync.Application.Exceptions;
using SeatSync.Domain.Entities;
using SeatSync.Domain.Enums;
using SeatSync.Infrastructure.Persistence;
namespace SeatSync.Infrastructure.Services;
public sealed class ReservationService(AppDbContext db, ICurrentUser currentUser, ISeatAvailabilityCache cache) : IReservationService
{
    public async Task<HoldResponse> CreateHoldAsync(Guid eventId, CreateHoldRequest request, string key, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(key)) throw new ConflictException("Idempotency-Key header is required.");
        var existing = await db.IdempotencyRecords.AsNoTracking().FirstOrDefaultAsync(x => x.UserId == currentUser.UserId && x.Key == key, ct);
        if (existing is not null) return await LoadHoldAsync(existing.HoldId, ct);
        await using var tx = await db.Database.BeginTransactionAsync(IsolationLevel.Serializable, ct);
        var seats = await db.Seats.Where(x => x.EventId == eventId && request.SeatIds.Contains(x.Id)).ToListAsync(ct);
        if (seats.Count != request.SeatIds.Count) throw new NotFoundException("One or more seats were not found.");
        var now = DateTime.UtcNow; if (seats.Any(x => !x.CanBeHeld(now))) throw new ConflictException("One or more seats are no longer available.");
        var hold = new SeatHold { EventId = eventId, UserId = currentUser.UserId, ExpiresAtUtc = now.AddMinutes(10) };
        foreach (var seat in seats) { seat.PlaceHold(hold.Id, hold.ExpiresAtUtc, now); hold.Items.Add(new SeatHoldItem { HoldId = hold.Id, SeatId = seat.Id }); }
        db.SeatHolds.Add(hold); db.IdempotencyRecords.Add(new IdempotencyRecord { UserId = currentUser.UserId, Key = key, HoldId = hold.Id });
        await db.SaveChangesAsync(ct); await tx.CommitAsync(ct); await cache.InvalidateAsync(eventId);
        return ToHold(hold);
    }

    public async Task<ReservationResponse> ConfirmHoldAsync(Guid holdId, CancellationToken ct)
    {
        await using var tx = await db.Database.BeginTransactionAsync(IsolationLevel.Serializable, ct);
        var hold = await db.SeatHolds.Include(x => x.Items).ThenInclude(x => x.Seat).FirstOrDefaultAsync(x => x.Id == holdId, ct) ?? throw new NotFoundException("Hold not found.");
        if (hold.UserId != currentUser.UserId) throw new ForbiddenException("This hold belongs to another user.");
        if (hold.Status != HoldStatus.Active || hold.ExpiresAtUtc <= DateTime.UtcNow) throw new ConflictException("Hold has expired or is no longer active.");
        var reservation = new Reservation { EventId = hold.EventId, UserId = currentUser.UserId };
        foreach (var item in hold.Items) { item.Seat.Confirm(hold.Id); reservation.Seats.Add(new ReservationSeat { ReservationId = reservation.Id, SeatId = item.SeatId, PriceAtBooking = item.Seat.Price }); }
        hold.Status = HoldStatus.Confirmed; db.Reservations.Add(reservation); await db.SaveChangesAsync(ct); await tx.CommitAsync(ct); await cache.InvalidateAsync(hold.EventId);
        return await LoadReservationAsync(reservation.Id, ct);
    }

    public async Task<IReadOnlyList<ReservationResponse>> GetMineAsync(CancellationToken ct)
    {
        var ids = await db.Reservations.AsNoTracking().Where(x => x.UserId == currentUser.UserId).OrderByDescending(x => x.CreatedAtUtc).Select(x => x.Id).ToListAsync(ct);
        var result = new List<ReservationResponse>(); foreach (var id in ids) result.Add(await LoadReservationAsync(id, ct)); return result;
    }

    public async Task CancelAsync(Guid reservationId, CancellationToken ct)
    {
        var reservation = await db.Reservations.Include(x => x.Seats).ThenInclude(x => x.Seat).FirstOrDefaultAsync(x => x.Id == reservationId, ct) ?? throw new NotFoundException("Reservation not found.");
        if (reservation.UserId != currentUser.UserId) throw new ForbiddenException("Reservation belongs to another user.");
        if (reservation.Status == ReservationStatus.Cancelled) return;
        reservation.Status = ReservationStatus.Cancelled; reservation.CancelledAtUtc = DateTime.UtcNow; foreach (var item in reservation.Seats) item.Seat.ReleaseReservation();
        await db.SaveChangesAsync(ct); await cache.InvalidateAsync(reservation.EventId);
    }

    private async Task<HoldResponse> LoadHoldAsync(Guid id, CancellationToken ct) => ToHold(await db.SeatHolds.AsNoTracking().Include(x => x.Items).FirstAsync(x => x.Id == id, ct));
    private static HoldResponse ToHold(SeatHold h) => new(h.Id, h.EventId, h.ExpiresAtUtc, h.Status.ToString(), h.Items.Select(x => x.SeatId).ToList());
    private async Task<ReservationResponse> LoadReservationAsync(Guid id, CancellationToken ct)
    {
        var r = await db.Reservations.AsNoTracking().Include(x => x.Seats).ThenInclude(x => x.Seat).FirstAsync(x => x.Id == id, ct);
        return new ReservationResponse(r.Id, r.EventId, r.Status.ToString(), r.CreatedAtUtc, r.Seats.Select(x => new ReservedSeatResponse(x.SeatId, x.Seat.Section, x.Seat.Row, x.Seat.Number, x.PriceAtBooking)).ToList());
    }
}
