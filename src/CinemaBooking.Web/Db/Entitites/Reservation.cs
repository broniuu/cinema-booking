namespace CinemaBooking.Web.Db.Entitites;

public class Reservation
{
    public required Guid Id { get; set; }
    public required string Name { get; set; }
    public required string Surname{ get; set; }
    public required string PhoneNumber { get; set; }
    public virtual Seat Seat { get; set; } = null!;
    public required Guid SeatId { get; set; }
    public virtual Screening Screening { get; set; } = null!;
    public required Guid ScreeningId { get; set; }
}
