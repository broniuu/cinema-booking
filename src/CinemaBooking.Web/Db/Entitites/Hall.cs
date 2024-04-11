namespace CinemaBooking.Web.Db.Entitites;

internal class Hall
{
    public int HallId { get; set; }
    public List<Seat> Seats { get; set; } = [];
    public int Name { get; set; }
    public Screening? Screening { get; set; }
}
