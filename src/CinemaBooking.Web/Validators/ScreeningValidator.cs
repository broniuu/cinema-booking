using CinemaBooking.Web.Db.Entitites;
using FluentValidation;

namespace CinemaBooking.Web.Validators;

public class ScreeningValidator : AbstractValidator<Screening>
{
    private readonly TimeProvider _timeProvider;
    public ScreeningValidator(TimeProvider timeProvider)
    {
        _timeProvider = timeProvider;
        RuleFor(s => s.HallId).NotEmpty();
        RuleFor(s => s.Name).NotEmpty();
        RuleFor(s => s.Date).Must(IsNow).WithMessage(s => $"Screening date: {s.Date:dd.mm.yyyy} can't be a past day");
        RuleFor(s => s.Id).NotEmpty();
    }

    private bool IsNow(DateOnly date) => date >= _timeProvider.GetLocalNow().ToDateOnly();
}
