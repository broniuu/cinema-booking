namespace CinemaBooking.Web.Db.Entitites;

public class Reservation
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public required string Surname{ get; set; }
    public required string PhoneNumber { get; set; }
    public virtual required Seat Seat { get; set; }
    public required Guid SeatId { get; set; }
    public virtual required Screening Screening { get; set; }
    public Guid ScreeningId { get; set; }
}
