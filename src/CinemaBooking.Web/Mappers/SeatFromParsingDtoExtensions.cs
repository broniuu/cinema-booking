using CinemaBooking.Seed.Dtos;
using CinemaBooking.Web.Db.Entitites;

namespace CinemaBooking.Web.Mappers;

public static class SeatFromParsingDtoExtensions
{
    public static Seat ToEntity(this SeatFromParsingDto dto, Hall hall) =>
        new()
        {
            Hall = hall,
            HallId = hall.Id,
            Id = dto.Id,
            PositionX = dto.PositionX,
            PositionY = dto.PositionY,
            SeatNumber = dto.SeatNumber
        };
}
