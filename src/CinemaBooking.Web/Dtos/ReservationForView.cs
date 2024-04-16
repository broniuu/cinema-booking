namespace CinemaBooking.Web.Dtos;

public class ReservationForView
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public required string Surname { get; set; }
    public required string PhoneNumber { get; set; }
}
