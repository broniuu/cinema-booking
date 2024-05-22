using CinemaBooking.Web.Components.Pages.Screenings;
using CinemaBooking.Web.Db.Entitites;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace CinemaBooking.Web.Validators;

public class ReservationValidator : AbstractValidator<Reservation>
{
    public ReservationValidator(IStringLocalizer<ReservationValidator> localizer)
    {
        RuleFor(r => r.Id).NotEmpty();
        RuleFor(r => r.Name).NotEmpty().WithMessage(localizer["NameNotEmpty"]);
        RuleFor(r => r.Surname).NotEmpty().WithMessage(localizer["SurnameNotEmpty"]);
        RuleFor(r => r.PhoneNumber).Matches(@"^\s*\+?\d+(?:\s\d+)*\s*").WithMessage(r => $"{r.PhoneNumber} {localizer["BadPhoneNumber"]}");
        RuleFor(r => r.ScreeningId).NotEmpty();
        RuleFor(r => r.SeatId).NotEmpty();
    }
}
