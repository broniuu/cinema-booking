using CinemaBooking.Web.Db;
using CinemaBooking.Web.Dtos;
using CinemaBooking.Web.Mappers;
using LanguageExt.Common;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace CinemaBooking.Web.Services;

public class HallViewService(IDbContextFactory<CinemaDbContext> dbContextFactory)
{
    private readonly IDbContextFactory<CinemaDbContext> dbContextFactory = dbContextFactory;

    public async Task<Result<HallForView>> GetHallViewAsync(Guid hallId)
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        var hall = await dbContext.Halls.FindAsync(hallId);
        if (hall is null)
        {
            var error = new ArgumentException($"Hall with Id: {hallId} doesn't exists");
            return new Result<HallForView>(error);
        }
        var seatsByRow = await dbContext.Seats.Where(s => s.HallId == hallId).GroupBy(s => s.PositionY).ToListAsync();
        var rowsForView = seatsByRow.Select(r => new SeatsRowForView([.. r.OrderBy(s => s.PositionX).Select(r => r.CreateForView())]));
        return new HallForView(hall.Name, [..rowsForView]);
    }
}
