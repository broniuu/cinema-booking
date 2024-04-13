namespace CinemaBooking.Web.Dtos;

public record SeatForView(Guid Id, int PositionX, int PositionY, string SeatNumber)
{
    public ReservationForView? Reservation { get; set; }
}
