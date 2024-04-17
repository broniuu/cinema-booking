﻿using Blazored.Toast;
using CinemaBooking.Web.Db;
using CinemaBooking.Web.Db.Entitites;
using CinemaBooking.Web.Services;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace CinemaBooking.Web;

public static class Config
{
    public static IServiceCollection AddCinemaManagementServices(this IServiceCollection services) => services
        .AddScoped<HallViewService>()
        .AddScoped<ScreeningService>()
        .AddScoped<GuidService>();

    public static IServiceCollection AddValidators(this IServiceCollection services) => services
        .AddScoped<IValidator<Screening>, ScreeningValidator>();

    public static IServiceCollection AddDbContextFactory(this IServiceCollection services) => services
        .AddDbContextFactory<CinemaDbContext>(o =>
            {
                string dbPathDirectoryPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "CinemaBooking");
                Directory.CreateDirectory(dbPathDirectoryPath);
                var dbPath = Path.Combine(dbPathDirectoryPath, "cinemaBookingData.db");
                o.UseSqlite($"Data Source={dbPath};");
            });
}
