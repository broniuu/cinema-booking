using CinemaBooking.Web.Db.Entitites;
using CinemaBooking.Web.Services;
using CinemaBooking.Web.UnitTests.TestHelpers;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using System.Globalization;
using Xunit.Sdk;

namespace CinemaBooking.Web.UnitTests.Services;
public class ScreeningService_GetAllAsyncTest : IDisposable
{
    public InMemorySqliteProvider SqliteProvider { get; private set; }

    public ScreeningService_GetAllAsyncTest()
    {
        SqliteProvider = new InMemorySqliteProvider();
        SqliteProvider.InitializeConnection();
    }

    public void Dispose()
    {
        SqliteProvider.DisposeConnection();
    }

    [Fact]
    public async Task WhenGettingScreenigns_RetrunAll()
    {
        await using var dbContext = SqliteProvider.CreateDbContext();
        var hall = new Hall()
        {
            Name = "MCK",
            Screenings = [],
            Seats = [],
            Id = Guid.Parse("7afe7b1b-945d-4cb5-afb0-c9fc06cca0ba")
        };
        await dbContext.Halls.AddAsync(hall);
        dbContext.Screenings.AddRange([
            new Screening() {
                Name = "Lord of the rings",
                Date = DateOnly.Parse("2025-03-11"),
                HallId = hall.Id,
                Id = Guid.Parse("766d8862-bd3e-4199-ae8a-4a273369f6a3")
            },
            new Screening() {
                Name = "Fast and Furious",
                Date = DateOnly.Parse("2025-03-15"),
                HallId = hall.Id,
                Id = Guid.Parse("4095989d-69b8-424f-b8a3-f46151744c21")
            },
            new Screening() {
                Name = "Star Wars",
                Date = DateOnly.Parse("2025-05-22"),
                HallId = hall.Id,
                Id = Guid.Parse("46fff3be-5e69-4c27-9418-fa8f1f4f53a4")
            },
            new Screening() {
                Name = "Dogs",
                Date = DateOnly.Parse("2025-05-28"),
                HallId = hall.Id,
                Id = Guid.Parse("8989b12d-d9be-475f-8ad6-10f856abbcfe")
            },
            ]);
        await dbContext.SaveChangesAsync();
        var validator = Substitute.For<IValidator<Screening>>();
        var screeningService = new ScreeningService(SqliteProvider.CreateDbContextFactory(), validator);
        var screeningsForView = await screeningService.GetAllAsync();
        screeningsForView.Should().BeEquivalentTo(
            [
                new {
                    Name = "Lord of the rings",
                    Date = DateTimeOffset.Parse("2025-03-11"),
                    HallId = hall.Id,
                    Id = Guid.Parse("766d8862-bd3e-4199-ae8a-4a273369f6a3")
                },
                new {
                    Name = "Fast and Furious",
                    Date = DateTimeOffset.Parse("2025-03-15"),
                    HallId = hall.Id,
                    Id = Guid.Parse("4095989d-69b8-424f-b8a3-f46151744c21")
                },
                new {
                    Name = "Star Wars",
                    Date = DateTimeOffset.Parse("2025-05-22"),
                    HallId = hall.Id,
                    Id = Guid.Parse("46fff3be-5e69-4c27-9418-fa8f1f4f53a4")
                },
                new {
                    Name = "Dogs",
                    Date = DateTimeOffset.Parse("2025-05-28"),
                    HallId = hall.Id,
                    Id = Guid.Parse("8989b12d-d9be-475f-8ad6-10f856abbcfe")
                }
            ]);
    }
}
