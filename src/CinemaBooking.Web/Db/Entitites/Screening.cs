namespace CinemaBooking.Web.Db.Entitites;

public class Screening
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public DateTimeOffset Date { get; set; }
    public List<Reservation> Reservations { get; set; } = [];
    public required Hall Hall { get; set; }
    public Guid HallId { get; set; }
}
