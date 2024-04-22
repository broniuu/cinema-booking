using CinemaBooking.Web.Db;
using CinemaBooking.Web.Db.Entitites;
using CinemaBooking.Web.Dtos;
using CinemaBooking.Web.Mappers;
using FluentValidation;
using LanguageExt.Common;
using Microsoft.EntityFrameworkCore;

namespace CinemaBooking.Web.Services;

public class ScreeningService(IDbContextFactory<CinemaDbContext> dbContextFactory, IValidator<Screening> screeningValidator, GuidService guidService)
{
    private readonly IDbContextFactory<CinemaDbContext> _dbContextFactory = dbContextFactory;
    private readonly IValidator<Screening> _screeningValidator = screeningValidator;
    private readonly GuidService _guidService = guidService;

    public async Task<List<ScreeningForView>> GetAllAsync()
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        return await dbContext.Screenings.Select(s => s.CreateForView()).ToListAsync();
    }

    public async Task<Result<ScreeningForView?>> AddAsync(AddScreeningDto addScreeningDto)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        var entityToAdd = addScreeningDto.CreateEntity(_guidService);
        var validationResult = await _screeningValidator.ValidateAsync(entityToAdd);
        if (!validationResult.IsValid)
        {
            return new Result<ScreeningForView?>(new ValidationException(validationResult.Errors));
        }
        await dbContext.Screenings.AddAsync(entityToAdd);
        await dbContext.SaveChangesAsync();
        return entityToAdd.CreateForView();
    }
}
