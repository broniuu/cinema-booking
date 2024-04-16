namespace CinemaBooking.Web.Dtos;

public class AddScreeningDto
{
    public string Name { get; set; }
    public DateTimeOffset Date { get; set; }
    public required Guid HallId { get; set; }
}
