using CinemaBooking.Web.Db.Entitites;
using CinemaBooking.Web.Services;
using CinemaBooking.Web.UnitTests.TestHelpers;
using Microsoft.Extensions.Logging;

namespace CinemaBooking.Web.UnitTests.Services;
public sealed class HallService_GetHallAsyncTest : IDisposable
{
    private readonly InMemorySqliteProvider _sqliteProvider;

    public HallService_GetHallAsyncTest()
    {
        _sqliteProvider = new InMemorySqliteProvider();
        _sqliteProvider.InitializeConnection();
    }

    [Fact]
    public async Task WhenHallNotExists_ThenReturnFaulted()
    {
        var logger = Substitute.For<ILogger<HallService>>();
        var hallViewService = new HallService(_sqliteProvider.CreateDbContextFactory(), logger);
        var result = await hallViewService.GetHallAsync();
        logger.ReceivedLogError("Halls contains no elements");
        result.IsFaulted.Should().BeTrue();
    }

    [Fact]
    public async Task WhenMoreThanOneHallExists_ThenReturnFaulted()
    {
        await using var dbContext = _sqliteProvider.CreateDbContext();
        await dbContext.Halls.AddRangeAsync(
            new Hall
            {
                Id = Guid.Parse("838c0861-96c2-47aa-ae59-fc9021fb9847"),
                Name = "fake hall 1"
            },
            new Hall
            {
                Id = Guid.Parse("0be3381e-4cd1-4601-883b-f32318320c4d"),
                Name = "fake hall 2"
            });
        await dbContext.SaveChangesAsync();
        var logger = Substitute.For<ILogger<HallService>>();
        var hallViewService = new HallService(_sqliteProvider.CreateDbContextFactory(), logger);
        var result = await hallViewService.GetHallAsync();
        logger.ReceivedLogError<InvalidOperationException>("Halls contains more than one element");
        result.IsFaulted.Should().BeTrue();
    }

    [Fact]
    public async Task WhenExactlyOneHallExists_ThenReturnIt()
    {
        await using var dbContext = _sqliteProvider.CreateDbContext();
        await dbContext.Halls.AddAsync(
            new Hall
            {
                Id = Guid.Parse("838c0861-96c2-47aa-ae59-fc9021fb9847"),
                Name = "fake hall 1"
            });
        await dbContext.SaveChangesAsync();
        var hallViewService = new HallService(_sqliteProvider.CreateDbContextFactory(), Substitute.For<ILogger<HallService>>());
        var result = await hallViewService.GetHallAsync();
        var hall = result.Match(h => h, _ => null);
        hall.Should().BeEquivalentTo(
            new
            {
                Id = Guid.Parse("838c0861-96c2-47aa-ae59-fc9021fb9847"),
                Name = "fake hall 1"
            });
    }
    public void Dispose()
    {
        _sqliteProvider.DisposeConnection();
    }
}
