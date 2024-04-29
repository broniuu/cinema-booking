using CinemaBooking.Web.Db.Entitites;
using CinemaBooking.Web.Services;
using CinemaBooking.Web.UnitTests.TestHelpers;
using FluentValidation.Results;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

namespace CinemaBooking.Web.UnitTests.Services;
public sealed class ReservationService_DeleteAsyncTest : IDisposable
{
    private readonly InMemorySqliteProvider _sqliteProvider;

    public ReservationService_DeleteAsyncTest()
    {
        _sqliteProvider = new InMemorySqliteProvider();
        _sqliteProvider.InitializeConnection();
    }

    public void Dispose()
    {
        _sqliteProvider.DisposeConnection();
    }

    [Fact]
    public async Task WhenExceptionOccuredWhileSaving_ThenRetrunException()
    {
        // Given
        var dbContextFactory = _sqliteProvider.CreateFakeFailedDbContextFactory();
        var validator = Substitute.For<IValidator<Reservation>>();
        validator.ValidateAsync(Arg.Any<Reservation>())
            .Returns(new ValidationResult(new List<ValidationFailure>()));
        var logger = Substitute.For<ILogger<ReservationService>>();
        var sut = new ReservationService(dbContextFactory, validator, logger);
        await using var dbContext = _sqliteProvider.CreateDbContext();
        await dbContext.Halls.AddAsync(new Hall()
        {
            Id = Guid.Parse("5eb6c229-4993-47df-83c1-4780b073ebb8"),
            Name = "MCK",
            Screenings =
            [
                new Screening()
                {
                    Id = Guid.Parse("99af86aa-5b7a-4dbd-a702-cfbec90f744a"),
                    HallId = Guid.Parse("5eb6c229-4993-47df-83c1-4780b073ebb8"),
                    Name = "test screening",
                    Date = DateOnly.Parse("2023-06-04"),
                }
            ],
            Seats = [
                new Seat() {
                    Id = Guid.Parse("30c185a7-3063-43fe-96d0-5f1edd36dd6e"),
                    HallId = Guid.Parse("5eb6c229-4993-47df-83c1-4780b073ebb8"),
                    SeatNumber = "2"
                }
            ]

        });
        await dbContext.SaveChangesAsync();
        var fakeReservation = new Reservation()
        {
            Id = Guid.Parse("99af86aa-5b7a-4dbd-a702-cfbec90f744a"),
            Name = "Jan",
            Surname = "Kowalski",
            ScreeningId = Guid.Parse("99af86aa-5b7a-4dbd-a702-cfbec90f744a"),
            SeatId = Guid.Parse("30c185a7-3063-43fe-96d0-5f1edd36dd6e"),
            PhoneNumber = "  +48 111 111 121  "
        };
        await dbContext.AddAsync(fakeReservation);
        await dbContext.SaveChangesAsync();
        //When
        var result = await sut.RemoveAsync(fakeReservation);
        //Then
        result.ShouldBeFaultedWithMessage("Error occured while removing reservation");
        logger.ReceivedLogError<DbUpdateConcurrencyException>("Error occured while removing reservation");
    }

    [Fact]
    public async Task WhenRemovingEntity_ThenRemoveItFromDb()
    {
        // Given
        var dbContextFactory = _sqliteProvider.CreateDbContextFactory();
        var validator = Substitute.For<IValidator<Reservation>>();
        validator.ValidateAsync(Arg.Any<Reservation>())
            .Returns(new ValidationResult(new List<ValidationFailure>()));
        var logger = Substitute.For<ILogger<ReservationService>>();
        var sut = new ReservationService(dbContextFactory, validator, logger);
        await using var dbContext = _sqliteProvider.CreateDbContext();
        await dbContext.Halls.AddAsync(new Hall()
        {
            Id = Guid.Parse("5eb6c229-4993-47df-83c1-4780b073ebb8"),
            Name = "MCK",
            Screenings =
            [
                new Screening()
                {
                    Id = Guid.Parse("99af86aa-5b7a-4dbd-a702-cfbec90f744a"),
                    HallId = Guid.Parse("5eb6c229-4993-47df-83c1-4780b073ebb8"),
                    Name = "test screening",
                    Date = DateOnly.Parse("2023-06-04"),
                }
            ],
            Seats = [
                new Seat() {
                    Id = Guid.Parse("30c185a7-3063-43fe-96d0-5f1edd36dd6e"),
                    HallId = Guid.Parse("5eb6c229-4993-47df-83c1-4780b073ebb8"),
                    SeatNumber = "2"
                }
            ]

        });
        await dbContext.SaveChangesAsync();
        var fakeReservation = new Reservation()
        {
            Id = Guid.Parse("99af86aa-5b7a-4dbd-a702-cfbec90f744a"),
            Name = "Jan",
            Surname = "Kowalski",
            ScreeningId = Guid.Parse("99af86aa-5b7a-4dbd-a702-cfbec90f744a"),
            SeatId = Guid.Parse("30c185a7-3063-43fe-96d0-5f1edd36dd6e"),
            PhoneNumber = "  +48 111 111 121  "
        };
        await dbContext.AddAsync(fakeReservation);
        await dbContext.SaveChangesAsync();
        //When
        var result = await sut.RemoveAsync(fakeReservation);
        //Then
        result.IsSuccess.Should().BeTrue();
        dbContext.Reservations.Should().BeEmpty();
    }
}
