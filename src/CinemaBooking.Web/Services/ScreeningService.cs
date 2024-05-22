using CinemaBooking.Web.Db;
using CinemaBooking.Web.Db.Entitites;
using FluentValidation;
using Result;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace CinemaBooking.Web.Services;

public class ScreeningService(
    IDbContextFactory<CinemaDbContext> dbContextFactory, 
    IValidator<Screening> validator, 
    ILogger<ScreeningService> logger,
    IStringLocalizer<ScreeningService> localizer)
{
    private readonly IDbContextFactory<CinemaDbContext> _dbContextFactory = dbContextFactory;
    private readonly IStringLocalizer<ScreeningService> _localizer = localizer;
    private readonly GenericCudService<Screening> _cudService = new(dbContextFactory, validator, logger);


    public async Task<List<Screening>> GetAllAsync()
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        return await dbContext.Screenings.OrderBy(s => s.Date).ToListAsync();
    }

    public Task<Result<Screening?>> AddAsync(Screening screening)
        => _cudService.AddAsync(screening, _localizer["ErrorAdd"]);

    public Task<Result<Screening?>> UpdateAsync(Screening screening)
        => _cudService.UpdateAsync(screening, _localizer["ErrorUpdate"]);

    public Task<Result<bool>> RemoveAsync(Screening screening)
        => _cudService.RemoveAsync(screening, _localizer["ErrorRemove"]);
}
