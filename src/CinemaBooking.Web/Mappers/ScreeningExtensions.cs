using CinemaBooking.Web.Db.Entitites;
using CinemaBooking.Web.Dtos;
using CinemaBooking.Web.Services;

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

    public static Screening CreateEntity(this AddScreeningDto screening, GuidService guidService) => new()
    {
        Name = screening.Name,
        Id = guidService.NewGuid(),
        HallId = screening.HallId,
        Date = screening.Date,
    };
}
