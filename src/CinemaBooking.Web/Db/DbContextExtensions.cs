using CinemaBooking.Seed;
using CinemaBooking.Web.Db.Entitites;
using CinemaBooking.Web.Mappers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Reflection;
using System.Text.Json;

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
        try
        {
            var pathToSeedFile = Path.Combine(webHostEnvironment.ContentRootPath, @"Db\hall-seats.seed.csv");
            var seatsFromParsing = SeatsParser.Parse(pathToSeedFile);
            var hall = await dbContext.Halls.FirstOrDefaultAsync();
            if (hall is null)
            {
                hall = new Hall() { Name = "MCK Bobowa" };
                await dbContext.Halls.AddAsync(hall);
            }
            if (!await dbContext.Seats.AnyAsync())
            {
                var seats = seatsFromParsing.Select(s => s.ToEntity(hall));
                await dbContext.Seats.AddRangeAsync(seats);
            }
            await dbContext.SaveChangesAsync();
        } catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while seeding data.");
        }
        return app;
    }
}
