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

    public async Task<Result<HallForView>> GetHallViewAsync(Guid hallId)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        var hall = await dbContext.Halls.FindAsync(hallId);
        if (hall is null)
        {
            _logger.LogError("Hall with Id: {HallId} doesn't exists", hallId);
            var error = new ArgumentException("Error occured while getting the halls");
            return new Result<HallForView>(error);
        }
        var seatsByRow = await dbContext.Seats.Where(s => s.HallId == hallId).GroupBy(s => s.PositionY).ToListAsync();
        var rowsForView = seatsByRow.Select(r => new SeatsRowForView([.. r.OrderBy(s => s.PositionX).Select(r => r.CreateForView())]));
        return new HallForView(hall.Name, [.. rowsForView]);
    }

    public async Task<Result<Hall?>> GetHallAsync()
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        var exceptionMessage = "Error occured when getting the hall";
        try
        {
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
