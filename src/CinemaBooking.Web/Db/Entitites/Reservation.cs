namespace CinemaBooking.Web.Db.Entitites;

internal class Revervation
{
    public int ReservationId { get; set; }
    public required string Name { get; set; }
    public required string Surname{ get; set; }
    public required string PhoneNumber { get; set; }
    public required Seat Seat { get; set; }
    public required Screening Screening { get; set; }
}
