using CinemaBooking.Web.Db.Entitites;
using CinemaBooking.Web.Services;
using CinemaBooking.Web.UnitTests.TestHelpers;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;

namespace CinemaBooking.Web.UnitTests.Services;
public sealed class ScreeningService_AddAsyncTest : IDisposable
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
        // Given
        var dbContextFactory = _sqliteProvider.CreateDbContextFactory();
        var validator = Substitute.For<IValidator<Screening>>();
        validator.ValidateAsync(Arg.Any<Screening>())
            .Returns(new ValidationResult(new List<ValidationFailure>()));
        var logger = Substitute.For<ILogger<ScreeningService>>();
        var sut = new ScreeningService(dbContextFactory, validator, logger);
        await using var dbContext = _sqliteProvider.CreateDbContext();
        await dbContext.Halls.AddAsync(new Hall()
        {
            Id = Guid.Parse("5eb6c229-4993-47df-83c1-4780b073ebb8"),
            Name = "MCK"
        });
        await dbContext.SaveChangesAsync();
        var fakeScreening = new Screening()
        {
            Id = Guid.Parse("99af86aa-5b7a-4dbd-a702-cfbec90f744a"),
            HallId = Guid.Parse("5eb6c229-4993-47df-83c1-4780b073ebb8"),
            Name = "test screening",
            Date = DateOnly.Parse("2023-06-04"),
        };
        //When
        var addedScreening = await sut.AddAsync(fakeScreening);
        //Then
        addedScreening.Match(s => s, _ => null).Should().BeEquivalentTo(
            new
            {
                HallId = Guid.Parse("5eb6c229-4993-47df-83c1-4780b073ebb8"),
                Name = "test screening",
                Date = DateOnly.Parse("2023-06-04"),
            }
        );
        dbContext.Screenings.Should().BeEquivalentTo(
            [
                new
                {
                    HallId = Guid.Parse("5eb6c229-4993-47df-83c1-4780b073ebb8"),
                    Name = "test screening",
                    Date = DateOnly.Parse("2023-06-04"),
                }
            ]
        );
        await validator.Received(1).ValidateAsync(Arg.Is<Screening>(
            s => IsEqual(s, new Screening()
            {
                Id = Guid.Parse("99af86aa-5b7a-4dbd-a702-cfbec90f744a"),
                HallId = Guid.Parse("5eb6c229-4993-47df-83c1-4780b073ebb8"),
                Date = DateOnly.Parse("2023-06-04"),
                Name = "test screening"
            })), default
        );
    }

    private static bool IsEqual(Screening screening1, Screening screening2) =>
        screening1.Id == screening2.Id
        && screening1.Name == screening2.Name
        && screening1.Date == screening2.Date
        && screening1.HallId == screening2.HallId;

    [Fact]
    public async Task WhenValidationFailed_ThenReturnError()
    {
        // Given
        var dbContextFactory = _sqliteProvider.CreateDbContextFactory();
        var validator = Substitute.For<IValidator<Screening>>();
        // Setup failure
        validator.ValidateAsync(Arg.Any<Screening>())
            .Returns(new ValidationResult(new List<ValidationFailure>() { new("", "first error message"), new("", "second error message") }));
        var logger = Substitute.For<ILogger<ScreeningService>>();
        var screeningService = new ScreeningService(dbContextFactory, validator, logger);
        await using var dbContext = _sqliteProvider.CreateDbContext();
        await dbContext.Halls.AddAsync(new Hall()
        {
            Id = Guid.Parse("5eb6c229-4993-47df-83c1-4780b073ebb8"),
            Name = "MCK"
        });
        await dbContext.SaveChangesAsync();
        var fakeScreening = new Screening()
        {
            Id = Guid.Empty,
            HallId = Guid.Parse("5eb6c229-4993-47df-83c1-4780b073ebb8"),
            Name = "test screening",
            Date = DateOnly.Parse("2023-06-04"),
        };
        // When
        var addedScreening = await screeningService.AddAsync(fakeScreening);
        // Then
        addedScreening.IfFail(e => e.Message.Should().Be("first error message"));
        addedScreening.Match(s => s, _ => null).Should().BeNull();
        dbContext.Screenings.Should().BeEquivalentTo(Array.Empty<object>());
        await validator.Received(1).ValidateAsync(Arg.Is<Screening>(
            s => IsEqual(s, new Screening()
            {
                Id = Guid.Empty,
                HallId = Guid.Parse("5eb6c229-4993-47df-83c1-4780b073ebb8"),
                Date = DateOnly.Parse("2023-06-04"),
                Name = "test screening"
            })), default
        );
    }

    [Fact]
    public async Task WhenExceptionOccuredWhileSaving_ThenRetrunException()
    {
        // Given
        var dbContextFactory = _sqliteProvider.CreateFakeFailedDbContextFactory();
        var validator = Substitute.For<IValidator<Screening>>();
        // Setup failure
        validator.ValidateAsync(Arg.Any<Screening>())
            .Returns(new ValidationResult(new List<ValidationFailure>()));
        var logger = Substitute.For<ILogger<ScreeningService>>();
        var screeningService = new ScreeningService(dbContextFactory, validator, logger);
        await using var dbContext = _sqliteProvider.CreateDbContext();
        await dbContext.Halls.AddAsync(new Hall()
        {
            Id = Guid.Parse("5eb6c229-4993-47df-83c1-4780b073ebb8"),
            Name = "MCK"
        });
        await dbContext.SaveChangesAsync();
        var fakeScreening = new Screening()
        {
            Id = Guid.Empty,
            HallId = Guid.Parse("5eb6c229-4993-47df-83c1-4780b073ebb8"),
            Name = "test screening",
            Date = DateOnly.Parse("2023-06-04"),
        };
        // When
        var addedScreeningResult = await screeningService.AddAsync(fakeScreening);
        // Then
        addedScreeningResult.ShouldBeFaultedWithMessage("Error occured while adding screening");
        dbContext.Screenings.Should().BeEquivalentTo(Array.Empty<object>());
    }

    public void Dispose()
    {
        _sqliteProvider.DisposeConnection();
    }
}
