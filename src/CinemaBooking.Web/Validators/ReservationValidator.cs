using CinemaBooking.Web.Db.Entitites;
using FluentValidation;

namespace CinemaBooking.Web.Validators;

public class ReservationValidator : AbstractValidator<Reservation>
{
    public ReservationValidator()
    {
        RuleFor(r => r.Id).NotEmpty();
        RuleFor(r => r.Name).NotEmpty();
        RuleFor(r => r.Surname).NotEmpty();
        RuleFor(r => r.PhoneNumber).Matches(@"^\s*\+?\d+(?:\s\d+)*\s*");
        RuleFor(r => r.ScreeningId).NotEmpty();
        RuleFor(r => r.SeatId).NotEmpty();
    }
}
