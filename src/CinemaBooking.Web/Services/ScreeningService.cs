using CinemaBooking.Web.Db;
using CinemaBooking.Web.Dtos;
using CinemaBooking.Web.Mappers;
using LanguageExt.Common;
using Microsoft.EntityFrameworkCore;

namespace CinemaBooking.Web.Services;

public class ScreeningService(IDbContextFactory<CinemaDbContext> dbContextFactory)
{
    private readonly IDbContextFactory<CinemaDbContext> dbContextFactory = dbContextFactory;
    public async Task<List<ScreeningForView>> GetAllAsync()
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        return await dbContext.Screenings.Select(s => s.CreateForView()).ToListAsync();
    }

    public async Task<Result<ScreeningForView>> AddAsync(AddScreeningDto addScreeningDto)
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        var addedScreening = await dbContext.Screenings.AddAsync(addScreeningDto.CreateEntity());
        await dbContext.SaveChangesAsync();
        return addedScreening.Entity.CreateForView();
    }
}
