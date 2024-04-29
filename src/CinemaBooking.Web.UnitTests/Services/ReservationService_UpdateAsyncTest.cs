using CinemaBooking.Web.Db.Entitites;
using CinemaBooking.Web.Services;
using CinemaBooking.Web.UnitTests.TestHelpers;
using FluentValidation.Results;
using FluentValidation;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace CinemaBooking.Web.UnitTests.Services;
public class ReservationService_UpdateAsyncTest : IDisposable
{
    private readonly InMemorySqliteProvider _sqliteProvider;

    public ReservationService_UpdateAsyncTest()
    {
        _sqliteProvider = new InMemorySqliteProvider();
        _sqliteProvider.InitializeConnection();
    }

    public void Dispose()
    {
        _sqliteProvider.DisposeConnection();
    }

    [Fact]
    public async Task WhenAddingEntity_ThenSaveItToDb()
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
        fakeReservation.Name = "Jack";
        fakeReservation.Surname = "Pstrong";
        //When
        var result = await sut.UpdateAsync(fakeReservation);
        //Then
        result.Match(s => s, _ => null).Should().BeEquivalentTo(
            new
            {
                Id = Guid.Parse("99af86aa-5b7a-4dbd-a702-cfbec90f744a"),
                Name = "Jack",
                Surname = "Pstrong",
                ScreeningId = Guid.Parse("99af86aa-5b7a-4dbd-a702-cfbec90f744a"),
                SeatId = Guid.Parse("30c185a7-3063-43fe-96d0-5f1edd36dd6e"),
                PhoneNumber = "+48111111121"
            }
        );
        dbContext.Reservations.Should().BeEquivalentTo(
            [
            new
            {
                Id = Guid.Parse("99af86aa-5b7a-4dbd-a702-cfbec90f744a"),
                Name = "Jack",
                Surname = "Pstrong",
                ScreeningId = Guid.Parse("99af86aa-5b7a-4dbd-a702-cfbec90f744a"),
                SeatId = Guid.Parse("30c185a7-3063-43fe-96d0-5f1edd36dd6e"),
                PhoneNumber = "+48111111121"
            }
            ]
        );
        await validator.Received(1).ValidateAsync(Arg.Is<Reservation>(
            s => IsEqual(s, fakeReservation)), default
        );
    }

    [Fact]
    public async Task WhenValidationFailed_ThenReturnError()
    {
        // Given
        var dbContextFactory = _sqliteProvider.CreateDbContextFactory();
        var validator = Substitute.For<IValidator<Reservation>>();
        validator.ValidateAsync(Arg.Any<Reservation>())
            .Returns(new ValidationResult(new List<ValidationFailure>() { new("prop1", "Error 1"), new("prop2", "Error 2") }));
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
            PhoneNumber = "+48111111121"
        };
        //When
        var result = await sut.UpdateAsync(fakeReservation);
        //Then
        result.ShouldBeFaultedWithMessage("Error 1");
        dbContext.Reservations.Should().BeEquivalentTo(Enumerable.Empty<object>());
        await validator.Received(1).ValidateAsync(Arg.Is<Reservation>(
            s => IsEqual(s, fakeReservation)), default
        );
        logger.ReceivedLogError("Error 1");
    }

    private static bool IsEqual(Reservation reservation1, Reservation reservation2) =>
    reservation1.Id == reservation2.Id
    && reservation1.Name == reservation2.Name
    && reservation1.Surname == reservation2.Surname
    && reservation1.PhoneNumber == reservation2.PhoneNumber
    && reservation1.SeatId == reservation2.SeatId
    && reservation1.ScreeningId == reservation2.ScreeningId;

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
            PhoneNumber = "+48111111121"
        };
        var addedEntityEntry = await dbContext.AddAsync(fakeReservation);
        addedEntityEntry.State = EntityState.Detached;
        addedEntityEntry.Entity.Name = "Jack";
        addedEntityEntry.Entity.Name = "Pstrong";
        await dbContext.SaveChangesAsync();
        //When
        var addedReservation = await sut.UpdateAsync(fakeReservation);
        //Then
        addedReservation.IsFaulted.Should().BeTrue();
        addedReservation.IfFail(e => e.Message.Should().Be("Error occured while updating reservation"));
        logger.ReceivedLogError<DbUpdateConcurrencyException>("Error occured while updating reservation");
    }
}
