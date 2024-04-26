using CinemaBooking.Web.Db;
using CinemaBooking.Web.Db.Entitites;
using CinemaBooking.Web.Dtos;
using CinemaBooking.Web.Mappers;
using LanguageExt.Common;
using Microsoft.EntityFrameworkCore;

namespace CinemaBooking.Web.Services;

public class HallService(IDbContextFactory<CinemaDbContext> dbContextFactory, ILogger<HallService> logger)
{
    private readonly IDbContextFactory<CinemaDbContext> _dbContextFactory = dbContextFactory;
    private readonly ILogger<HallService> _logger = logger;

    public async Task<Result<HallForView?>> GetHallViewAsync(Guid screeningId)
    {
        var exceptionMessage = "Error occured when getting the hall";
        try
        {
            await using var dbContext = await _dbContextFactory.CreateDbContextAsync();
            var hall = await dbContext.Halls.SingleOrDefaultAsync(); // DB should contain only one hall
            if (hall is null)
            {
                _logger.LogError("Halls contains no elements");
                return new Result<HallForView?>(new Exception(exceptionMessage));
            }
            var seatsByRow = await dbContext.Seats.Include(s => s.Reservations).Where(s => s.HallId == hall.Id).GroupBy(s => s.PositionY).ToListAsync();
            var rowsForView = seatsByRow.Select(r => new SeatsRowForView([.. r.OrderBy(s => s.PositionX).Select(s => s.CreateForView(screeningId))]));
            return new HallForView(hall.Name, [.. rowsForView]);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "Halls contains more than one element");
            return new Result<HallForView?>(new Exception(exceptionMessage));
        }
    }


    public async Task<Result<Hall?>> GetHallAsync()
    {
        var exceptionMessage = "Error occured when getting the hall";
        try
        {
            await using var dbContext = await _dbContextFactory.CreateDbContextAsync();
            var hall = await dbContext.Halls.SingleOrDefaultAsync(); // DB should contain only one hall
            if (hall is null)
            {
                _logger.LogError("Halls contains no elements");
                return new Result<Hall?>(new Exception(exceptionMessage));
            }
            return hall;
        } catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "Halls contains more than one element");
            return new Result<Hall?>(new Exception(exceptionMessage));
        }
}
}
