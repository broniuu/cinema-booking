using CinemaBooking.Web.Db.Entitites;
using CinemaBooking.Web.Services;
using CinemaBooking.Web.UnitTests.TestHelpers;
using FluentValidation;
using FluentValidation.Results;

namespace CinemaBooking.Web.UnitTests.Services;
public sealed class ReservationService_AddAsyncTest : IDisposable
{
    private readonly InMemorySqliteProvider _sqliteProvider;

    public ReservationService_AddAsyncTest()
    {
        _sqliteProvider = new InMemorySqliteProvider();
        _sqliteProvider.InitializeConnection();
    }

    [Fact]
    public async Task WhenAddingEntity_ThenSaveItToDb()
    {
        // Given
        var dbContextFactory = _sqliteProvider.CreateDbContextFactory();
        var validator = Substitute.For<IValidator<Reservation>>();
        validator.ValidateAsync(Arg.Any<Reservation>())
            .Returns(new ValidationResult(new List<ValidationFailure>()));
        var sut = new ReservationService(dbContextFactory, validator);
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
            PhoneNumber = "111111111111"
        };
        //When
        var addedReservation = await sut.AddAsync(fakeReservation);
        //Then
        addedReservation.Match(s => s, _ => null).Should().BeEquivalentTo(
            new
            {
                Id = Guid.Parse("99af86aa-5b7a-4dbd-a702-cfbec90f744a"),
                Name = "Jan",
                Surname = "Kowalski",
                ScreeningId = Guid.Parse("99af86aa-5b7a-4dbd-a702-cfbec90f744a"),
                SeatId = Guid.Parse("30c185a7-3063-43fe-96d0-5f1edd36dd6e"),
                PhoneNumber = "111111111111"
            }
        );
        dbContext.Reservations.Should().BeEquivalentTo(
            [
            new
            {
                Id = Guid.Parse("99af86aa-5b7a-4dbd-a702-cfbec90f744a"),
                Name = "Jan",
                Surname = "Kowalski",
                ScreeningId = Guid.Parse("99af86aa-5b7a-4dbd-a702-cfbec90f744a"),
                SeatId = Guid.Parse("30c185a7-3063-43fe-96d0-5f1edd36dd6e"),
                PhoneNumber = "111111111111"
            }
            ]
        );
        await validator.Received(1).ValidateAsync(Arg.Is<Reservation>(
            s => IsEqual(s, fakeReservation)), default
        );
    }

    private static bool IsEqual(Reservation reservation1, Reservation reservation2) =>
        reservation1.Id == reservation2.Id
        && reservation1.Name == reservation2.Name
        && reservation1.Surname == reservation2.Surname
        && reservation1.PhoneNumber == reservation2.PhoneNumber
        && reservation1.SeatId == reservation2.SeatId
        && reservation1.ScreeningId == reservation2.ScreeningId;

    [Fact]
    public async Task WhenValidationFailed_ThenReturnError()
    {
        // Given
        var dbContextFactory = _sqliteProvider.CreateDbContextFactory();
        var validator = Substitute.For<IValidator<Reservation>>();
        // Setup failure
        validator.ValidateAsync(Arg.Any<Reservation>())
            .Returns(new ValidationResult(new List<ValidationFailure>() { new("", "first error message"), new("", "second error message") }));
        var sut = new ReservationService(dbContextFactory, validator);
        await using var dbContext = _sqliteProvider.CreateDbContext();
        await dbContext.Halls.AddAsync(new Hall()
        {
            Id = Guid.Parse("5eb6c229-4993-47df-83c1-4780b073ebb8"),
            Name = "MCK"
        });
        await dbContext.SaveChangesAsync();
        var fakeReservation = new Reservation()
        {
            Id = Guid.Parse("99af86aa-5b7a-4dbd-a702-cfbec90f744a"),
            Name = "Jan",
            Surname = "Kowalski",
            ScreeningId = Guid.Parse("99af86aa-5b7a-4dbd-a702-cfbec90f744a"),
            SeatId = Guid.Parse("30c185a7-3063-43fe-96d0-5f1edd36dd6e"),
            PhoneNumber = "111111111111"
        };
        // When
        var addedScreening = await sut.AddAsync(fakeReservation);
        // Then
        addedScreening.IfFail(e => e.Message.Should().Be("first error message"));
        addedScreening.Match(s => s, _ => null).Should().BeNull();
        dbContext.Screenings.Should().BeEquivalentTo(Array.Empty<object>());
        await validator.Received(1).ValidateAsync(Arg.Is<Reservation>(
            r => IsEqual(r, new Reservation()
            {
                Id = Guid.Parse("99af86aa-5b7a-4dbd-a702-cfbec90f744a"),
                Name = "Jan",
                Surname = "Kowalski",
                ScreeningId = Guid.Parse("99af86aa-5b7a-4dbd-a702-cfbec90f744a"),
                SeatId = Guid.Parse("30c185a7-3063-43fe-96d0-5f1edd36dd6e"),
                PhoneNumber = "111111111111"
            })), default
        );
    }

    public void Dispose()
    {
        _sqliteProvider.DisposeConnection();
    }
}

