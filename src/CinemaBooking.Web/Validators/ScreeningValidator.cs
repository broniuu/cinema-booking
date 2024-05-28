using CinemaBooking.Web.Db.Entitites;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace CinemaBooking.Web.Validators;

public class ScreeningValidator : AbstractValidator<Screening>
{
    private readonly TimeProvider _timeProvider;
    public ScreeningValidator(TimeProvider timeProvider, IStringLocalizer<ScreeningValidator> localizer)
    {
        _timeProvider = timeProvider;
        RuleFor(s => s.HallId).NotEmpty();
        RuleFor(s => s.Name).NotEmpty().WithMessage(localizer["NameNotEmpty"]);
        RuleFor(s => s.Date).Must(IsNow).WithMessage(s => $"{localizer["ScreeningDate"]} {s.Date} {localizer["CantBePast"]}");
        RuleFor(s => s.Id).NotEmpty();
    }

    private bool IsNow(DateOnly date) => date >= _timeProvider.GetLocalNow().ToDateOnly();
}
