using FluentValidation;
using System;

namespace CinemaBooking.Web.Db.Entitites;

public class Screening
{
    public required Guid Id { get; set; }
    public required string Name { get; set; }
    public DateTimeOffset Date { get; set; }
    public List<Reservation> Reservations { get; set; } = [];
    public virtual Hall Hall { get; set; } = null!;
    public required Guid HallId { get; set; }
}

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

    private bool IsNow(DateTimeOffset date) => date.Date >= _timeProvider.GetLocalNow().Date;
}