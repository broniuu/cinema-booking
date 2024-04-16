using System.Collections.Immutable;

namespace CinemaBooking.Web.Dtos;

public record HallForView(string Name, IReadOnlyCollection<SeatsRowForView> SeatsRows)
{
    
}
