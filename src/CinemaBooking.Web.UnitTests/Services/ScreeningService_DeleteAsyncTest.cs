using CinemaBooking.Web.Db.Entitites;
using CinemaBooking.Web.Services;
using FluentValidation.Results;
using FluentValidation;
using CinemaBooking.Web.UnitTests.TestHelpers;

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
        var sut = new ScreeningService(dbContextFactory, validator);
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
            Date = DateTimeOffset.Parse("2023-06-04"),
        };
        await dbContext.Screenings.AddAsync(fakeScreening);
        await dbContext.SaveChangesAsync();
        //When
        await sut.RemoveAsync(fakeScreening);
        //Then
        dbContext.Screenings.Should().BeEquivalentTo(Enumerable.Empty<object>());
    }
}
