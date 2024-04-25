using CinemaBooking.Web.Db;
using CinemaBooking.Web.Db.Entitites;
using FluentValidation;
using LanguageExt.Common;
using Microsoft.EntityFrameworkCore;

namespace CinemaBooking.Web.Services;

public class ScreeningService(IDbContextFactory<CinemaDbContext> dbContextFactory, IValidator<Screening> validator)
    : GenericCudService<Screening>(dbContextFactory, validator)
{
    public async Task<List<Screening>> GetAllAsync()
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        return await dbContext.Screenings.OrderBy(s => s.Date).ToListAsync();
    }
}
