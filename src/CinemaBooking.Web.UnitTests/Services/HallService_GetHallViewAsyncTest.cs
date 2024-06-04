using CinemaBooking.Web.Db.Entitites;
using CinemaBooking.Web.Services;
using CinemaBooking.Web.UnitTests.TestHelpers;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

namespace CinemaBooking.Web.UnitTests.Services;
public sealed class HallService_GetHallViewAsyncTest : IDisposable
{
    private readonly InMemorySqliteProvider _sqliteProvider;

    public HallService_GetHallViewAsyncTest()
    {
        _sqliteProvider = new InMemorySqliteProvider();
        _sqliteProvider.InitializeConnection();
    }

    [Fact]
    public async Task WhenMoreThanOneHallExists_ThenReturnFaulted()
    {
        await using var dbContext = _sqliteProvider.CreateDbContext();
        await dbContext.Halls.AddRangeAsync(
            new Hall()
            {
                Id = Guid.Parse("caa6d6b1-c32c-466e-bad6-e272e103889c"),
                Name = "Test 1"
            },
            new Hall()
            {
                Id = Guid.Parse("ffbc38c0-ce9e-4b49-a74c-8c541fb78db7"),
                Name = "Test 2"
            });
        await dbContext.SaveChangesAsync();
        var logger = Substitute.For<ILogger<HallService>>();
        var localizer = Substitute.For<IStringLocalizer<HallService>>();
        localizer["ErrorGetting"].Returns(new LocalizedString("ErrorGetting", "Error occured when getting the hall"));
        var hallViewService = new HallService(_sqliteProvider.CreateDbContextFactory(), logger, localizer);
        var result = await hallViewService.GetHallViewAsync(Guid.Parse("815c9457-5edf-48df-bf0a-37d5981c0fbe"));
        logger.ReceivedLogError<InvalidOperationException>("Halls contains more than one element");
        result.IsFaulted.Should().BeTrue();
        result.IfFail(e => e.Message.Should().Be("Error occured when getting the hall"));
    }

    [Fact]
    public async Task WhenHallExists_CreateHallForView()
    {
        var dbContext = _sqliteProvider.CreateDbContext();
        var hall = new Hall()
        {
            Name = "Test",
            Id = Guid.Parse("114b855e-7325-4b60-ade2-8bb83871751a"),
            Screenings = [
                new() {
                    Id = Guid.Parse("815c9457-5edf-48df-bf0a-37d5981c0fbe"),
                    Name = "Test name",
                    Date = DateOnly.Parse("2023-06-04"),
                    HallId = Guid.Parse("114b855e-7325-4b60-ade2-8bb83871751a"),
                },
                new() {
                    Id = Guid.Parse("b46ebe09-7879-42e9-bb02-e234da7c0275"),
                    Name = "Test name 2",
                    Date = DateOnly.Parse("2023-06-04"),
                    HallId = Guid.Parse("114b855e-7325-4b60-ade2-8bb83871751a")
                }
                ]
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
                Reservations = [
                    new() {
                        Id = Guid.Parse("75a595e5-3632-4f96-b20a-feff1ed5876e"),
                        Name = "John",
                        Surname = "Kowalski",
                        SeatId = Guid.Parse("fe2bad47-5c36-4c5e-82a2-0fc887c01687"),
                        PhoneNumber = "11",
                        ScreeningId = Guid.Parse("815c9457-5edf-48df-bf0a-37d5981c0fbe")
                    },
                    new() {
                        Id = Guid.Parse("2de1b023-a180-4525-b4ac-b75727170619"),
                        Name = "Adam",
                        Surname = "Białowieski",
                        SeatId = Guid.Parse("fe2bad47-5c36-4c5e-82a2-0fc887c01687"),
                        PhoneNumber = "22",
                        ScreeningId = Guid.Parse("b46ebe09-7879-42e9-bb02-e234da7c0275")
                    },
                    ]
            },
        ]);
        await dbContext.SaveChangesAsync();
        var localizer = Substitute.For<IStringLocalizer<HallService>>();
        localizer["ErrorGetting"].Returns(new LocalizedString("ErrorGetting", "Error occured when getting the hall"));
        var hallViewService = new HallService(_sqliteProvider.CreateDbContextFactory(), Substitute.For<ILogger<HallService>>(), localizer);
        var result = await hallViewService.GetHallViewAsync(Guid.Parse("b46ebe09-7879-42e9-bb02-e234da7c0275"));
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
                                Reservation = (object?)null
                            },
                            new
                            {
                                Id = Guid.Parse("d8106666-7c7b-4850-bd5e-bf9a4e866fde"),
                                SeatNumber = "2",
                                PositionX = 1,
                                PositionY = 0,
                                Reservation = (object?)null
                            },
                            new
                            {
                                Id = Guid.Parse("1adc9984-8d25-48cc-bbf0-6aa8518c17f3"),
                                SeatNumber = "3",
                                PositionX = 2,
                                PositionY = 0,
                                Reservation = (object?)null
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
                                Reservation = (object?)null
                            },
                            new
                            {
                                Id = Guid.Parse("8e64143a-b833-42f6-a3d6-9e31400a5d22"),
                                SeatNumber = "5",
                                PositionX = 1,
                                PositionY = 1,
                                Reservation = (object?)null
                            },
                            new
                            {
                                Id = Guid.Parse("f2fce476-f73f-4061-bf0f-9c0f3f857ab6"),
                                SeatNumber = "6",
                                PositionX = 2,
                                PositionY = 1,
                                Reservation = (object?)null
                            },
                            new
                            {
                                Id = Guid.Parse("fe2bad47-5c36-4c5e-82a2-0fc887c01687"),
                                SeatNumber = "7",
                                PositionX = 3,
                                PositionY = 1,
                                Reservation = (object?)new {
                                    Id = Guid.Parse("2de1b023-a180-4525-b4ac-b75727170619"),
                                    Name = "Adam",
                                    Surname = "Białowieski",
                                    SeatId = Guid.Parse("fe2bad47-5c36-4c5e-82a2-0fc887c01687"),
                                    PhoneNumber = "22",
                                    ScreeningId = Guid.Parse("b46ebe09-7879-42e9-bb02-e234da7c0275")
                                },
                            },
                        }
                    }
                }
            },
            o => o.ExcludingMissingMembers()
            );
    }

    public void Dispose() => _sqliteProvider.DisposeConnection();
}
