using CinemaBooking.Web.Db.Entitites;

namespace CinemaBooking.Web.Dtos;

public class ScreeningForView
{
    public required Guid Id { get; set; }
    public required string Name { get; set; }
    public required DateTimeOffset Date { get; set; }
    public required Guid HallId { get; set; }
}
