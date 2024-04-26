using CinemaBooking.Web.Db.Entitites;
using CinemaBooking.Web.Db;
using FluentValidation;
using LanguageExt.Common;
using Microsoft.EntityFrameworkCore;

namespace CinemaBooking.Web.Services;

public class GenericCudService<TEntity>(IDbContextFactory<CinemaDbContext> dbContextFactory, IValidator<TEntity> validator) where TEntity : class
{
    protected readonly IDbContextFactory<CinemaDbContext> _dbContextFactory = dbContextFactory;
    protected readonly IValidator<TEntity> _validator = validator;

    public virtual async Task<Result<TEntity?>> AddAsync(TEntity entity)
    {
        var validationResult = await _validator.ValidateAsync(entity);
        if (!validationResult.IsValid)
        {
            var errorMessage = validationResult.Errors.First().ErrorMessage;
            return new Result<TEntity?>(new Exception(errorMessage));
        }
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        var addedEntityEntry = await dbContext.Set<TEntity>().AddAsync(entity);
        await dbContext.SaveChangesAsync();
        return addedEntityEntry.Entity;
    }

    public virtual async Task<Result<TEntity?>> UpdateAsync(TEntity entity)
    {
        var validationResult = await _validator.ValidateAsync(entity);
        if (!validationResult.IsValid)
        {
            var errorMessage = validationResult.Errors.First().ErrorMessage;
            return new Result<TEntity?>(new Exception(errorMessage));
        }
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        dbContext.Entry(entity).State = EntityState.Modified;
        await dbContext.SaveChangesAsync();
        return entity;
    }

    public virtual async Task RemoveAsync(TEntity screening)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        dbContext.Set<TEntity>().Remove(screening);
        await dbContext.SaveChangesAsync();
    }
}
