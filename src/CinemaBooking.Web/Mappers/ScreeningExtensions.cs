using CinemaBooking.Web.Db.Entitites;
using CinemaBooking.Web.Dtos;

namespace CinemaBooking.Web.Mappers;

public static class ScreeningExtensions
{
    public static ScreeningForView CreateForView(this Screening screening) => new()
    {
        Name = screening.Name,
        Id = screening.Id,
        HallId = screening.HallId,
        Date = screening.Date,
    };

    public static Screening CreateEntity(this AddScreeningDto screening) => new()
    {
        Name = screening.Name,
        Id = Guid.NewGuid(),
        HallId = screening.HallId,
        Date = screening.Date,
    };
}
