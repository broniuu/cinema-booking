using CinemaBooking.Web.Db.Entitites;
using CinemaBooking.Web.Dtos;

namespace CinemaBooking.Web.Mappers;

public static class SeatExtensions
{
    public static SeatForView CreateForView(this Seat seat) =>
        new(seat.Id, seat.PositionX, seat.PositionY, seat.SeatNumber, seat.IsForDisabled)
        {
            Reservation = seat.Reservation?.CreateForView()
        };
}
