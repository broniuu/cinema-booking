using Microsoft.EntityFrameworkCore;

namespace CinemaBooking.Web.Db;

internal static class DbContextExtensions
{
    internal static async Task<IServiceProvider> CreateAndFillDbAsync(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateAsyncScope();
        var dbContextFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<CinemaDbContext>>();
        using var dbContext = await dbContextFactory.CreateDbContextAsync();
        await dbContext.Database.EnsureCreatedAsync();
        return serviceProvider;
    }
}
