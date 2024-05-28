using CinemaBooking.Web.Db.Entitites;
using CinemaBooking.Web.Services;
using FluentValidation.Results;
using FluentValidation;
using CinemaBooking.Web.UnitTests.TestHelpers;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace CinemaBooking.Web.UnitTests.Services;
public sealed class ScreeningService_DeleteAsyncTest : IDisposable
{
    private readonly InMemorySqliteProvider _sqliteProvider;

    public ScreeningService_DeleteAsyncTest()
    {
        _sqliteProvider = new InMemorySqliteProvider();
        _sqliteProvider.InitializeConnection();
    }
    public void Dispose()
    {
        _sqliteProvider.DisposeConnection();
    }

    [Fact]
    public async Task WhenDeletingEntity_ThenRemoveItFromDb()
    {
        // Given
        var dbContextFactory = _sqliteProvider.CreateDbContextFactory();
        var validator = Substitute.For<IValidator<Screening>>();
        validator.ValidateAsync(Arg.Any<Screening>())
            .Returns(new ValidationResult(new List<ValidationFailure>()));
        var logger = Substitute.For<ILogger<ScreeningService>>();
        var localizer = Substitute.For<IStringLocalizer<ScreeningService>>();
        var sut = new ScreeningService(dbContextFactory, validator, logger, localizer);
        await using var dbContext = _sqliteProvider.CreateDbContext();
        await dbContext.Halls.AddAsync(new Hall()
        {
            Id = Guid.Parse("5eb6c229-4993-47df-83c1-4780b073ebb8"),
            Name = "MCK"
        });
        var fakeScreening = new Screening()
        {
            Id = Guid.Parse("99af86aa-5b7a-4dbd-a702-cfbec90f744a"),
            HallId = Guid.Parse("5eb6c229-4993-47df-83c1-4780b073ebb8"),
            Name = "test screening",
            Date = DateOnly.Parse("2023-06-04"),
        };
        await dbContext.Screenings.AddAsync(fakeScreening);
        await dbContext.SaveChangesAsync();
        //When
        var result = await sut.RemoveAsync(fakeScreening);
        //Then
        result.IsSuccess.Should().BeTrue();
        dbContext.Screenings.Should().BeEquivalentTo(Enumerable.Empty<object>());
    }

    [Fact]
    public async Task WhenExceptionOccuredWhileSaving_ThenRetrunException()
    {
        // Given
        var dbContextFactory = _sqliteProvider.CreateFakeFailedDbContextFactory();
        var validator = Substitute.For<IValidator<Screening>>();
        validator.ValidateAsync(Arg.Any<Screening>())
            .Returns(new ValidationResult(new List<ValidationFailure>()));
        var logger = Substitute.For<ILogger<ScreeningService>>();
        var localizer = Substitute.For<IStringLocalizer<ScreeningService>>();
        localizer["ErrorRemove"].Returns(new LocalizedString("ErrorRemove", "Error occured while removing screening"));
        var sut = new ScreeningService(dbContextFactory, validator, logger, localizer);
        await using var dbContext = _sqliteProvider.CreateDbContext();
        await dbContext.Halls.AddAsync(new Hall()
        {
            Id = Guid.Parse("5eb6c229-4993-47df-83c1-4780b073ebb8"),
            Name = "MCK"
        });
        var fakeScreening = new Screening()
        {
            Id = Guid.Parse("99af86aa-5b7a-4dbd-a702-cfbec90f744a"),
            HallId = Guid.Parse("5eb6c229-4993-47df-83c1-4780b073ebb8"),
            Name = "test screening",
            Date = DateOnly.Parse("2023-06-04"),
        };
        await dbContext.Screenings.AddAsync(fakeScreening);
        await dbContext.SaveChangesAsync();
        //When
        var result = await sut.RemoveAsync(fakeScreening);
        //Then
        result.ShouldBeFaultedWithMessage("Error occured while removing screening");
        dbContext.Screenings.Should().BeEquivalentTo([fakeScreening]);
        logger.ReceivedLogError<DbUpdateConcurrencyException>("Error occured while removing screening");
    }
}
