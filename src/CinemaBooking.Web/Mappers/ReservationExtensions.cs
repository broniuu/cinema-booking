using CinemaBooking.Web.Db.Entitites;
using CinemaBooking.Web.Dtos;

namespace CinemaBooking.Web.Mappers;

public static class ReservationExtensions
{
    public static ReservationForView CreateForView(this Reservation reservation) =>
        new()
        {
            Id = reservation.Id,
            Name = reservation.Name,
            Surname = reservation.Surname,
            PhoneNumber = reservation.PhoneNumber,
        };
}
