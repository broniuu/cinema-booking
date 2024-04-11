using Microsoft.EntityFrameworkCore;

namespace CinemaBooking.Web.Db;

internal static class DbContextExtensions
{
    internal static async Task<IServiceProvider> CreateAndMigrateDbAsync(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateAsyncScope();
        var dbContextFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<CinemaDbContext>>();
        var dbContext = await dbContextFactory.CreateDbContextAsync();
        await dbContext.Database.MigrateAsync();
        return serviceProvider;
    }
}
