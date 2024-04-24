using CinemaBooking.Seed;
using CinemaBooking.Web.Db.Entitites;
using CinemaBooking.Web.Mappers;
using CinemaBooking.Web.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;

namespace CinemaBooking.Web.Db;

internal static class DbContextExtensions
{
    internal static async Task<IApplicationBuilder> MigrateDbAsync(this IApplicationBuilder app)
    {
        using var dbContext = await CreateDbContextAsync(app);
        await dbContext.Database.MigrateAsync();
        return app;
    }

    private static Task<CinemaDbContext> CreateDbContextAsync(IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateAsyncScope();
        var dbContextFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<CinemaDbContext>>();
        return dbContextFactory.CreateDbContextAsync();
    }

    internal static async Task<IApplicationBuilder> FillInDatabaseAsync(this IApplicationBuilder app, ILogger logger)
    {
        using var scope = app.ApplicationServices.CreateAsyncScope();
        var webHostEnvironment = scope.ServiceProvider.GetRequiredService<IWebHostEnvironment>();
        var dbContextFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<CinemaDbContext>>();
        var dbContext = await dbContextFactory.CreateDbContextAsync();
        await dbContext.FillInHallsAsync(logger, webHostEnvironment);
        await dbContext.FillInScreeningsAsync(logger);
        return app;
    }

    private static async Task<CinemaDbContext> FillInHallsAsync(this CinemaDbContext dbContext, ILogger logger, IWebHostEnvironment webHostEnvironment)
    {
        try
        {
            var pathToSeedFile = Path.Combine(webHostEnvironment.ContentRootPath, @"Db\hall-seats.seed.csv");
            var seatsFromParsing = SeatsParser.Parse(pathToSeedFile);
            var hall = await dbContext.Halls.FirstOrDefaultAsync();
            if (hall is null)
            {
                hall = new Hall() { Name = "MCK Bobowa", Id = Guid.Parse("865f86e6-5a3d-4fbf-970f-98f8f15484f3") };
                await dbContext.Halls.AddAsync(hall);
            }
            if (!await dbContext.Seats.AnyAsync())
            {
                var seats = seatsFromParsing.Select(s => s.ToEntity(hall));
                await dbContext.Seats.AddRangeAsync(seats);
            }
            await dbContext.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while seeding data.");
        }
        return dbContext;
    }

    private static async Task<CinemaDbContext> FillInScreeningsAsync(this CinemaDbContext dbContext, ILogger logger)
    {
        try
        {
            var hall = await dbContext.Halls.FirstOrDefaultAsync();
            if (hall is null)
            {
                logger.LogError("Can't create Screenings, because Hall is null");
                return dbContext;
            }
            if (await dbContext.Screenings.AnyAsync())
            {
                return dbContext;
            }
            var now = DateOnly.FromDateTime(DateTime.UtcNow);
            await dbContext.Screenings.AddRangeAsync(
                [
                    new() {
                        Id = Guid.NewGuid(),
                        Date = now.AddDays(1),
                        HallId = hall.Id,
                        Name = "Lord of the rings"
                    },
                    new() {
                        Id = Guid.NewGuid(),
                        Date = now.AddDays(10),
                        HallId = hall.Id,
                        Name = "Star wars"
                    },
                    new() {
                        Id = Guid.NewGuid(),
                        Date = now.AddDays(22),
                        HallId = hall.Id,
                        Name = "Yesterday"
                    },
                    new() {
                        Id = Guid.NewGuid(),
                        Date = now.AddDays(37),
                        HallId = hall.Id,
                        Name = "Gladiator"
                    },
                ]);
            await dbContext.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while seeding data.");
        }
        return dbContext;
    }
}
