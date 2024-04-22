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

