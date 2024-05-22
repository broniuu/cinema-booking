using CinemaBooking.Seed.Dtos;
using CinemaBooking.Web.Db;
using CinemaBooking.Web.Db.Entitites;
using CinemaBooking.Web.Services;
using CinemaBooking.Web.Services.Parsing;
using CinemaBooking.Web.UnitTests.TestHelpers;
using Result;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CinemaBooking.Web.UnitTests.Services.Parsing;
public sealed class ParseSeatsService_SaveSeatsFromTempFileToDbAsyncTest : IDisposable
{
    private const long MaxFileSize = 1024 * 15;
    private readonly InMemorySqliteProvider _sqliteProvider;
    public ParseSeatsService_SaveSeatsFromTempFileToDbAsyncTest()
    {
        _sqliteProvider = new InMemorySqliteProvider();
        _sqliteProvider.InitializeConnection();
    }

    [Fact]
    public async Task WhenTempFileNotExists_ReturnException()
    {
        var seatsParser = GetParserMock();
        var logger = GetLoggerMock();
        var dbContextFactory = GetDbContextFactoryMock();
        var appDataService = GetAppDataServiceMock();
        var guidService = GetGuidServiceMock();
        using var sut = new ParseSeatsService(dbContextFactory, logger, seatsParser, appDataService, guidService);

        var result = await sut.SaveSeatsFromTempFileToDbAsync("fake hall", ",");
        result.ShouldBeFaultedWithMessage("Unexpected error occured when saving seats");
        logger.ReceivedLogErrorWithStackTrace($"Temporary file does not exists");

    }

    [Fact]
    public async Task WhenErrorOccuredWhileParsing_ReturnException()
    {
        var seatsParser = GetParserMock();
        seatsParser.Parse(Arg.Any<string>(), Arg.Any<string>()).Returns(new Result<List<SeatFromParsingDto>?>(new Exception("fake error")));
        var logger = GetLoggerMock();
        var dbContextFactory = GetDbContextFactoryMock();
        var appDataService = GetAppDataServiceMock();
        appDataService.GetTemporaryCsvFolderPath().Returns("");
        var guidService = GetGuidServiceMock();
        guidService.NewGuid().Returns(Guid.Parse("f4f0d6b0-b04b-4b04-8d17-285b05dc8774"));
        using var sut = new ParseSeatsService(dbContextFactory, logger, seatsParser, appDataService, guidService);
        var fakeBrowserFile = Substitute.For<IBrowserFile>();
        using var stream = string.Empty.ToStream();
        fakeBrowserFile.OpenReadStream(Arg.Any<long>()).Returns(stream);
        await sut.CopyToTemporaryFileAsync(fakeBrowserFile);
        var result = await sut.SaveSeatsFromTempFileToDbAsync("fake hall", ",");
        result.ShouldBeFaultedWithMessage("fake error");
    }

    [Fact]
    public async Task WhenErrorOccuredWhileSavingToDb_ReturnException()
    {
        var seatsParser = GetParserMock();
        seatsParser.Parse(Arg.Any<string>(), Arg.Any<string>()).Returns(new Result<List<SeatFromParsingDto>?>([]));
        var logger = GetLoggerMock();
        var dbContextFactory = _sqliteProvider.CreateFakeFailedDbContextFactory();
        var appDataService = GetAppDataServiceMock();
        appDataService.GetTemporaryCsvFolderPath().Returns("");
        var guidService = GetGuidServiceMock();
        guidService.NewGuid().Returns(Guid.Parse("5e34da89-9809-4870-9f0f-86131b009b39"));
        using var sut = new ParseSeatsService(dbContextFactory, logger, seatsParser, appDataService, guidService);
        using var stream = string.Empty.ToStream();
        var fakeBrowserFile = Substitute.For<IBrowserFile>();
        fakeBrowserFile.OpenReadStream(MaxFileSize).Returns(stream);
        await sut.CopyToTemporaryFileAsync(fakeBrowserFile);
        var result = await sut.SaveSeatsFromTempFileToDbAsync("fake hall", ",");
        result.ShouldBeFaultedWithMessage("Unexpected error occured when saving seats");
        logger.ReceivedLogError(new DbUpdateConcurrencyException("concurency error"));
    }

    [Fact]
    public async Task WhenNoExceptions_SaveSeatsToDb()
    {
        // Setup DB
        using var dbContext = _sqliteProvider.CreateDbContext();
        await dbContext.Halls.AddAsync(
            new Hall
            {
                Id = Guid.Parse("98cefb38-f73f-4820-ac71-feeb49e358ec"),
                Name = "Hall 1",
                Seats = [
                    new() {
                            Id = Guid.Parse("44427ca6-191b-44dd-8bf6-f070a5a738b1"),
                            HallId = Guid.Parse("98cefb38-f73f-4820-ac71-feeb49e358ec"),
                            SeatNumber = "1"
                    },
                    new() {
                        Id = Guid.Parse("434febb9-1203-4aa1-a0f8-3400f4b3eaeb"),
                        HallId = Guid.Parse("98cefb38-f73f-4820-ac71-feeb49e358ec"),
                        SeatNumber = "1"
                    },
                    new() {
                        Id = Guid.Parse("bdbcecdc-845d-4d91-a3ae-71cf94e4df36"),
                        HallId = Guid.Parse("98cefb38-f73f-4820-ac71-feeb49e358ec"),
                        SeatNumber = "1"
                    },
                    new() {
                        Id = Guid.Parse("5c20624f-e9cb-453c-ad1e-c3f8b13dfbb8"),
                        HallId = Guid.Parse("98cefb38-f73f-4820-ac71-feeb49e358ec"),
                        SeatNumber = "1"
                    },
                    new() {
                        Id = Guid.Parse("3957af75-4891-4157-b53a-d10f59e42d0a"),
                        HallId = Guid.Parse("98cefb38-f73f-4820-ac71-feeb49e358ec"),
                        SeatNumber = "1"
                    },
                ]
            }
        );
        await dbContext.SaveChangesAsync();
        // Setup parser
        var seatsParser = GetParserMock();
        seatsParser.Parse(Arg.Any<string>(), Arg.Any<string>()).Returns(new Result<List<SeatFromParsingDto>?>([
            new SeatFromParsingDto() {
                SeatNumber = "1",
                PositionX = 1,
                PositionY = 1,
                Id = Guid.Parse("0e3785f9-409a-4bc9-8246-cfcc498855cf"),
                IsForDisabled = false,
            },
            new SeatFromParsingDto() {
                SeatNumber = "2",
                PositionX = 1,
                PositionY = 2,
                Id = Guid.Parse("9db5a4e8-bda9-452e-9421-343090dbe87f"),
                IsForDisabled = true,
            },
            new SeatFromParsingDto() {
                SeatNumber = "3",
                PositionX = 1,
                PositionY = 3,
                Id = Guid.Parse("537a236f-ad6e-4b14-83b9-5e61214c4e17"),
                IsForDisabled = false,
            },
            new SeatFromParsingDto() {
                SeatNumber = "4",
                PositionX = 2,
                PositionY = 1,
                Id = Guid.Parse("f5989699-200f-4351-8c72-28f9938428ec"),
                IsForDisabled = false,
            }
            ]));
        var logger = GetLoggerMock();
        var dbContextFactory = _sqliteProvider.CreateDbContextFactory();
        var appDataService = GetAppDataServiceMock();
        appDataService.GetTemporaryCsvFolderPath().Returns("");
        var guidService = GetGuidServiceMock();
        guidService.NewGuid().Returns(Guid.Parse("deb4d624-aa4b-4152-88ba-b9bc19637fa5"), Guid.Parse("d5e369b9-10d1-4fba-9d40-b9a53ae1dbf3"));
        using var sut = new ParseSeatsService(dbContextFactory, logger, seatsParser, appDataService, guidService);
        using var stream = string.Empty.ToStream();
        var fakeBrowserFile = Substitute.For<IBrowserFile>();
        fakeBrowserFile.OpenReadStream(MaxFileSize).Returns(stream);
        await sut.CopyToTemporaryFileAsync(fakeBrowserFile);
        var result = await sut.SaveSeatsFromTempFileToDbAsync("fake hall", ",");
        result.IsSuccess.Should().BeTrue();
        dbContext.Halls.Should().BeEquivalentTo(new[]
        {
            new
            {
                Id = Guid.Parse("d5e369b9-10d1-4fba-9d40-b9a53ae1dbf3"),
                Name = "fake hall"
            }
        });
        dbContext.Seats.Should().BeEquivalentTo(new[]
        {
            new  {
                SeatNumber = "1",
                PositionX = 1,
                PositionY = 1,
                Id = Guid.Parse("0e3785f9-409a-4bc9-8246-cfcc498855cf"),
                IsForDisabled = false,
            },
            new {
                SeatNumber = "2",
                PositionX = 1,
                PositionY = 2,
                Id = Guid.Parse("9db5a4e8-bda9-452e-9421-343090dbe87f"),
                IsForDisabled = true,
            },
            new {
                SeatNumber = "3",
                PositionX = 1,
                PositionY = 3,
                Id = Guid.Parse("537a236f-ad6e-4b14-83b9-5e61214c4e17"),
                IsForDisabled = false,
            },
            new {
                SeatNumber = "4",
                PositionX = 2,
                PositionY = 1,
                Id = Guid.Parse("f5989699-200f-4351-8c72-28f9938428ec"),
                IsForDisabled = false,
            }
        });


    }

    private static SeatsParser GetParserMock() => Substitute.For<SeatsParser>(Substitute.For<ILogger<SeatsParser>>());
    private static ILogger<ParseSeatsService> GetLoggerMock() => Substitute.For<ILogger<ParseSeatsService>>();
    private static IDbContextFactory<CinemaDbContext> GetDbContextFactoryMock() => Substitute.For<IDbContextFactory<CinemaDbContext>>();
    private static AppDataService GetAppDataServiceMock() => Substitute.For<AppDataService>();
    private static GuidService GetGuidServiceMock() => Substitute.For<GuidService>();

    public void Dispose()
    {
        _sqliteProvider.DisposeConnection();
    }
}
