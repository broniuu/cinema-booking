namespace CinemaBooking.Web.Db.Entitites;

internal class Screening
{
    public int ScreeningId { get; set; }
    public required string Name { get; set; }
    public DateTimeOffset Date { get; set; }
    public List<Seat> Reservations { get; set; } = [];
    public required Hall Hall { get; set; }
}
