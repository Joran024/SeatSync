using FluentValidation;
using SeatSync.Application.Contracts.Holds;
namespace SeatSync.Application.Validation;
public sealed class CreateHoldRequestValidator : AbstractValidator<CreateHoldRequest>
{
    public CreateHoldRequestValidator()
    {
        RuleFor(x => x.SeatIds).NotEmpty().Must(x => x.Count <= 6).WithMessage("A hold can contain at most 6 seats.");
        RuleFor(x => x.SeatIds).Must(x => x.Distinct().Count() == x.Count).WithMessage("Seat ids must be unique.");
    }
}
