using CinemaBooking.Web.Db.Entitites;
using CinemaBooking.Web.Services;
using CinemaBooking.Web.UnitTests.TestHelpers;
using FluentAssertions;

namespace CinemaBooking.Web.UnitTests.Services;
public class HallViewServiceGetTest
{
    private readonly InMemorySqliteProvider _sqliteProvider;

    public HallViewServiceGetTest()
    {
        _sqliteProvider = new InMemorySqliteProvider();
        _sqliteProvider.InitializeConnection();
    }



    [Fact]
    public async Task WhenHallNotExists_ThenThrowException()
    {
        var dbContext = _sqliteProvider.CreateDbContext();
        await dbContext.Halls.AddAsync(
            new()
            {
                Name = "Test",
                Screenings = [],
                Seats = [],
                Id = Guid.Parse("3f83d3e6-9bcb-447e-8979-b5bcae4a19da"),
            }

        );
        await dbContext.SaveChangesAsync();
        var hallViewService = new HallViewService(_sqliteProvider.CreateDbContextFactory());
        var result = await hallViewService.GetHallViewAsync(Guid.Parse("85a91b48-b1d9-400b-b3ba-a96cbbaed499"));
        result.IsFaulted.Should().BeTrue();
    }

    [Fact]
    public async Task WhenHallExists_CreateHallForView()
    {
        var dbContext = _sqliteProvider.CreateDbContext();
        var hall = new Hall()
        {
            Name = "Test",
            Id = Guid.Parse("114b855e-7325-4b60-ade2-8bb83871751a"),
        };
        await dbContext.Halls.AddAsync(hall);
        await dbContext.Seats.AddRangeAsync([
            new()
            {
                Id = Guid.Parse("c96a5fe4-b4ea-49b8-a86f-4dcac1b26d8d"),
                SeatNumber = "1",
                PositionX = 0,
                PositionY = 0,
                Hall = hall,
                HallId = hall.Id,
            },
            new()
            {
                Id = Guid.Parse("d8106666-7c7b-4850-bd5e-bf9a4e866fde"),
                SeatNumber = "2",
                PositionX = 1,
                PositionY = 0,
                Hall = hall,
                HallId = hall.Id,
            },
            new()
            {
                Id = Guid.Parse("1adc9984-8d25-48cc-bbf0-6aa8518c17f3"),
                SeatNumber = "3",
                PositionX = 2,
                PositionY = 0,
                Hall = hall,
                HallId = hall.Id,
            },
            new()
            {
                Id = Guid.Parse("7675c02b-202f-4280-9b7d-86085a83f120"),
                SeatNumber = "4",
                PositionX = 0,
                PositionY = 1,
                Hall = hall,
                HallId = hall.Id,
            },
            new()
            {
                Id = Guid.Parse("8e64143a-b833-42f6-a3d6-9e31400a5d22"),
                SeatNumber = "5",
                PositionX = 1,
                PositionY = 1,
                Hall = hall,
                HallId = hall.Id,
            },
            new()
            {
                Id = Guid.Parse("f2fce476-f73f-4061-bf0f-9c0f3f857ab6"),
                SeatNumber = "6",
                PositionX = 2,
                PositionY = 1,
                Hall = hall,
                HallId = hall.Id,
            },
            new()
            {
                Id = Guid.Parse("fe2bad47-5c36-4c5e-82a2-0fc887c01687"),
                SeatNumber = "7",
                PositionX = 3,
                PositionY = 1,
                Hall = hall,
                HallId = hall.Id,
            },
        ]);
        await dbContext.SaveChangesAsync();
        var hallViewService = new HallViewService(_sqliteProvider.CreateDbContextFactory());
        var result = await hallViewService.GetHallViewAsync(Guid.Parse("114b855e-7325-4b60-ade2-8bb83871751a"));
        var hallView = result.Match(hv => hv, e => default!);
        hallView.Should().BeEquivalentTo(
            new
            {
                Name = "Test",
                SeatsRows = new[]
                {
                    new
                    {
                        Seats = new[] //Row 0
                        {
                            new
                            {
                                Id = Guid.Parse("c96a5fe4-b4ea-49b8-a86f-4dcac1b26d8d"),
                                SeatNumber = "1",
                                PositionX = 0,
                                PositionY = 0,
                            },
                            new
                            {
                                Id = Guid.Parse("d8106666-7c7b-4850-bd5e-bf9a4e866fde"),
                                SeatNumber = "2",
                                PositionX = 1,
                                PositionY = 0,
                            },
                            new
                            {
                                Id = Guid.Parse("1adc9984-8d25-48cc-bbf0-6aa8518c17f3"),
                                SeatNumber = "3",
                                PositionX = 2,
                                PositionY = 0,
                            },
                        }
                    },
                    new
                    {
                        Seats = new[] //Row 1
                        {
                            new
                            {
                                Id = Guid.Parse("7675c02b-202f-4280-9b7d-86085a83f120"),
                                SeatNumber = "4",
                                PositionX = 0,
                                PositionY = 1,
                            },
                            new
                            {
                                Id = Guid.Parse("8e64143a-b833-42f6-a3d6-9e31400a5d22"),
                                SeatNumber = "5",
                                PositionX = 1,
                                PositionY = 1,
                            },
                            new
                            {
                                Id = Guid.Parse("f2fce476-f73f-4061-bf0f-9c0f3f857ab6"),
                                SeatNumber = "6",
                                PositionX = 2,
                                PositionY = 1,
                            },
                            new
                            {
                                Id = Guid.Parse("fe2bad47-5c36-4c5e-82a2-0fc887c01687"),
                                SeatNumber = "7",
                                PositionX = 3,
                                PositionY = 1,
                            },
                        }
                    }
                }
            },
            o => o.ExcludingMissingMembers()
            );
    }
}
