namespace CinemaBooking.Web.Db.Entitites;

public class Seat
{
    public Guid Id { get; set; }
    public int PositionX { get; set; }
    public int PositionY { get; set; }
    public required string SeatNumber { get; set; }
    public virtual required Hall Hall { get; set; }
    public virtual required Guid HallId { get; set; }
    public virtual Reservation? Reservation { get; set; }
}
