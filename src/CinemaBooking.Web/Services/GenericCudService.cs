using CinemaBooking.Web.Db.Entitites;
using CinemaBooking.Web.Db;
using FluentValidation;
using LanguageExt.Common;
using Microsoft.EntityFrameworkCore;

namespace CinemaBooking.Web.Services;

public class GenericCudService<TEntity>(
    IDbContextFactory<CinemaDbContext> dbContextFactory, 
    IValidator<TEntity> validator, 
    ILogger logger) where TEntity : class
{
    protected readonly IDbContextFactory<CinemaDbContext> _dbContextFactory = dbContextFactory;
    protected readonly IValidator<TEntity> _validator = validator;
    private readonly ILogger _logger = logger;

    public virtual async Task<Result<TEntity?>> AddAsync(TEntity entity, string dbCouncurencyErrorMessage)
    {
        var validationResult = await _validator.ValidateAsync(entity);
        if (!validationResult.IsValid)
        {
            var errorMessage = validationResult.Errors.First().ErrorMessage;
            _logger.LogError("{Message}", errorMessage);
            return new Result<TEntity?>(new Exception(errorMessage));
        }
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        var addedEntityEntry = await dbContext.Set<TEntity>().AddAsync(entity);
        return await SaveChangesAsync(dbContext, addedEntityEntry.Entity, dbCouncurencyErrorMessage);
    }

    private async Task<Result<T>> SaveChangesAsync<T>(CinemaDbContext dbContext, T succesfullResult, string errorMessage)
    {
        try
        {
            await dbContext.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "{Message}", errorMessage);
            return new Result<T>(new Exception(errorMessage));
        }
        return succesfullResult;
    }

    public virtual async Task<Result<TEntity?>> UpdateAsync(TEntity entity, string dbCouncurencyErrorMessage)
    {
        var validationResult = await _validator.ValidateAsync(entity);
        if (!validationResult.IsValid)
        {
            var errorMessage = validationResult.Errors.First().ErrorMessage;
            _logger.LogError("{Message}", errorMessage);
            return new Result<TEntity?>(new Exception(errorMessage));
        }
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        dbContext.Attach(entity);
        dbContext.Entry(entity).State = EntityState.Modified;
        return await SaveChangesAsync(dbContext, entity, dbCouncurencyErrorMessage);
    }

    public virtual async Task<Result<bool>> RemoveAsync(TEntity entity, string dbCouncurencyErrorMessage)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        dbContext.Attach(entity);
        dbContext.Set<TEntity>().Remove(entity);
        return await SaveChangesAsync(dbContext, true, dbCouncurencyErrorMessage);
    }

    public virtual async Task<Result<bool>> RemoveAsync(CinemaDbContext dbContext, TEntity entity, string dbCouncurencyErrorMessage)
    {
        dbContext.Attach(entity);
        dbContext.Set<TEntity>().Remove(entity);
        return await SaveChangesAsync(dbContext, true, dbCouncurencyErrorMessage);
    }
}
