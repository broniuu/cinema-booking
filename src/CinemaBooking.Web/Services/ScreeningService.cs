using CinemaBooking.Web.Db;
using CinemaBooking.Web.Db.Entitites;
using FluentValidation;
using LanguageExt.Common;
using Microsoft.EntityFrameworkCore;

namespace CinemaBooking.Web.Services;

public class ScreeningService(IDbContextFactory<CinemaDbContext> dbContextFactory, IValidator<Screening> validator)
{
    private readonly IDbContextFactory<CinemaDbContext> _dbContextFactory = dbContextFactory;
    private readonly IValidator<Screening> _validator = validator;

    public async Task<List<Screening>> GetAllAsync()
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        return await dbContext.Screenings.ToListAsync();
    }

    public async Task<Result<Screening?>> AddAsync(Screening screeningToAdd)
    {
        var validationResult = await _validator.ValidateAsync(screeningToAdd);
        if (!validationResult.IsValid)
        {
            var errorMessage = validationResult.Errors.First().ErrorMessage;
            return new Result<Screening?>(new Exception(errorMessage));
        }
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        var addedScreeningEntry = await dbContext.Screenings.AddAsync(screeningToAdd);
        await dbContext.SaveChangesAsync();
        return addedScreeningEntry.Entity;
    }

    public async Task<Result<Screening?>> UpdateAsync(Screening screeningToUpdate)
    {
        var validationResult = await _validator.ValidateAsync(screeningToUpdate);
        if (!validationResult.IsValid)
        {
            var errorMessage = validationResult.Errors.First().ErrorMessage;
            return new Result<Screening?>(new Exception(errorMessage));
        }
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        dbContext.Entry(screeningToUpdate).State = EntityState.Modified;
        await dbContext.SaveChangesAsync();
        return screeningToUpdate;
    }

    public async Task RemoveAsync(Screening screening)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        dbContext.Screenings.Remove(screening);
        await dbContext.SaveChangesAsync();
    }
}
