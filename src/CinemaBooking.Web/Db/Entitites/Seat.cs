namespace CinemaBooking.Web.Db.Entitites;

internal class Seat
{
    public int SeetId { get; set; }
    public int PositionX { get; set; }
    public int PositionY { get; set; }
    public required string SeatNumber { get; set; }
    public required Hall Hall { get; set; }
    public List<Seat> Reservations { get; set; } = [];
}
