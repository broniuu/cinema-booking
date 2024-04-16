using CinemaBooking.Web.Db.Entitites;
using CinemaBooking.Web.Dtos;
using CinemaBooking.Web.Services;
using CinemaBooking.Web.UnitTests.TestHelpers;
using FluentAssertions;

namespace CinemaBooking.Web.UnitTests.Services;
public class ScreeningService_AddAsyncTest
{
    private readonly InMemorySqliteProvider _sqliteProvider;

    public ScreeningService_AddAsyncTest()
    {
        _sqliteProvider = new InMemorySqliteProvider();
        _sqliteProvider.InitializeConnection();
    }

    [Fact]
    public async Task WhenAddingEntity_ThenSaveItToDb()
    {
        var dbContextFactory = _sqliteProvider.CreateDbContextFactory();
        var screeningService = new ScreeningService(dbContextFactory);

        var fakeScreening = new AddScreeningDto()
        {
            HallId = Guid.Parse("5eb6c229-4993-47df-83c1-4780b073ebb8"),
            Name = "test screening",
            Date = DateTimeOffset.Parse("2023-06-04"),
        };
        await using var dbContext = _sqliteProvider.CreateDbContext();
        await dbContext.Halls.AddAsync(new Hall()
        {
            Id = Guid.Parse("5eb6c229-4993-47df-83c1-4780b073ebb8"),
            Name = "MCK"
        });
        await dbContext.SaveChangesAsync();
        var addedScreening = await screeningService.AddAsync(fakeScreening);
        addedScreening.Should().BeEquivalentTo(
            new
            {
                HallId = Guid.Parse("5eb6c229-4993-47df-83c1-4780b073ebb8"),
                Name = "test screening",
                Date = DateTimeOffset.Parse("2023-06-04"),
            }
        );
        dbContext.Screenings.Should().BeEquivalentTo(
            [
                new
                {
                    HallId = Guid.Parse("5eb6c229-4993-47df-83c1-4780b073ebb8"),
                    Name = "test screening",
                    Date = DateTimeOffset.Parse("2023-06-04"),
                }
            ]
        );
    }
}
