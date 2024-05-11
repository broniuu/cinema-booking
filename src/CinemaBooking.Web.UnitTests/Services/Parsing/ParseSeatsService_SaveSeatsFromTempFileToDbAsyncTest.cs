using CinemaBooking.Seed.Dtos;
using CinemaBooking.Web.Db;
using CinemaBooking.Web.Services;
using CinemaBooking.Web.Services.Parsing;
using CinemaBooking.Web.UnitTests.TestHelpers;
using LanguageExt.Common;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CinemaBooking.Web.UnitTests.Services.Parsing;
public sealed class ParseSeatsService_SaveSeatsFromTempFileToDbAsyncTest : IDisposable
{
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
        var result = await sut.SaveSeatsFromTempFileToDbAsync("fake hall", ",");
        result.ShouldBeFaultedWithMessage("Unexpected error occured when saving seats");
        logger.ReceivedLogError<Exception>("");
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
