using FluentAssertions;
using SeatSync.Domain.Entities;
using SeatSync.Domain.Enums;
namespace SeatSync.UnitTests;
public sealed class SeatTests
{
    [Fact] public void Available_seat_can_be_held()
    {
        var seat = new Seat(); var holdId = Guid.NewGuid(); var now = DateTime.UtcNow;
        seat.PlaceHold(holdId, now.AddMinutes(10), now);
        seat.Status.Should().Be(SeatStatus.Held); seat.ActiveHoldId.Should().Be(holdId);
    }
    [Fact] public void Active_hold_blocks_second_hold()
    {
        var seat = new Seat(); var now = DateTime.UtcNow; seat.PlaceHold(Guid.NewGuid(), now.AddMinutes(10), now);
        var action = () => seat.PlaceHold(Guid.NewGuid(), now.AddMinutes(10), now);
        action.Should().Throw<InvalidOperationException>();
    }
    [Fact] public void Expired_hold_can_be_replaced()
    {
        var seat = new Seat(); var now = DateTime.UtcNow; seat.PlaceHold(Guid.NewGuid(), now.AddMinutes(1), now); var later = now.AddMinutes(2); var next = Guid.NewGuid();
        seat.PlaceHold(next, later.AddMinutes(10), later); seat.ActiveHoldId.Should().Be(next);
    }
    [Fact] public void Only_matching_hold_can_confirm()
    {
        var seat = new Seat(); var hold = Guid.NewGuid(); var now = DateTime.UtcNow; seat.PlaceHold(hold, now.AddMinutes(10), now);
        var action = () => seat.Confirm(Guid.NewGuid()); action.Should().Throw<InvalidOperationException>(); seat.Confirm(hold); seat.Status.Should().Be(SeatStatus.Reserved);
    }
}
