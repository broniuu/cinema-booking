using CinemaBooking.Web.Db;
using CinemaBooking.Web.Db.Entitites;
using CinemaBooking.Web.Services;
using CinemaBooking.Web.Services.Parsing;
using CinemaBooking.Web.Validators;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace CinemaBooking.Web;

public static class Config
{
    public static IServiceCollection AddCinemaManagementServices(this IServiceCollection services) => services
        .AddScoped<HallService>()
        .AddScoped<ReservationService>()
        .AddScoped<ScreeningService>()
        .AddScoped<GuidService>()
        .AddScoped<SeatsParser>()
        .AddScoped<AppDataService>()
        .AddScoped<ParseSeatsService>();

    public static IServiceCollection AddValidators(this IServiceCollection services) => services
        .AddScoped<IValidator<Screening>, ScreeningValidator>()
        .AddScoped<IValidator<Reservation>, ReservationValidator>();

    public static IServiceCollection AddDbContextFactory(this IServiceCollection services) => services
        .AddDbContextFactory<CinemaDbContext>((sp, o )=>
            {
                var dbPathDirectoryPath = new AppDataService().GetAppDataPath();
                var dbPath = Path.Combine(dbPathDirectoryPath, "cinemaBookingData.db");
                o.UseSqlite($"Data Source={dbPath};");
            });
    public static IApplicationBuilder CreateAppDataDirectories(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var serviceProvider = scope.ServiceProvider;
        var appDataService = serviceProvider.GetRequiredService<AppDataService>();
        appDataService.CreateAppDataDirectories();
        return app;
    }
}
