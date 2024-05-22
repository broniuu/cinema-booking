using CinemaBooking.Web.Db;
using CinemaBooking.Web.Db.Entitites;
using FluentValidation;
using Result;
using Microsoft.EntityFrameworkCore;

namespace CinemaBooking.Web.Services;

public class ScreeningService(IDbContextFactory<CinemaDbContext> dbContextFactory, IValidator<Screening> validator, ILogger<ScreeningService> logger)
{
    private readonly IDbContextFactory<CinemaDbContext> _dbContextFactory = dbContextFactory;
    private readonly GenericCudService<Screening> _cudService = new(dbContextFactory, validator, logger);


    public async Task<List<Screening>> GetAllAsync()
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        return await dbContext.Screenings.OrderBy(s => s.Date).ToListAsync();
    }

    public Task<Result<Screening?>> AddAsync(Screening screening)
        => _cudService.AddAsync(screening, "Error occured while adding screening");

    public Task<Result<Screening?>> UpdateAsync(Screening screening)
        => _cudService.UpdateAsync(screening, "Error occured while updating screening");

    public Task<Result<bool>> RemoveAsync(Screening screening)
        => _cudService.RemoveAsync(screening, "Error occured while removing screening");
}
