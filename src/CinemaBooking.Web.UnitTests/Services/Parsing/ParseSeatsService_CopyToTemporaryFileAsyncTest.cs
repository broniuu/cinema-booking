using CinemaBooking.Web.Db;
using CinemaBooking.Web.Services;
using CinemaBooking.Web.Services.Parsing;
using CinemaBooking.Web.UnitTests.TestHelpers;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using NSubstitute.ExceptionExtensions;

namespace CinemaBooking.Web.UnitTests.Services.Parsing;
public class ParseSeatsService_CopyToTemporaryFileAsyncTest
{
    private const long MaxFileSize = 1024 * 15;
    [Fact]
    public async Task WhenNoException_ThenCopyToTemporaryFile()
    {
        var seatsParser = GetParserMock();
        var logger = GetLoggerMock();
        var dbContextFactory = GetDbContextFactoryMock();
        var appDataService = GetAppDataServiceMock();
        appDataService.GetTemporaryCsvFolderPath().Returns("");
        var guidService = GetGuidServiceMock();
        guidService.NewGuid().Returns(Guid.Parse("fc1fa5bb-b10c-481b-98ac-6f99a17cffcb"));
        var fakeTextToCopy = """
                1,2,3,4,5,6,7
                8,9,10
                11,12,13,14,15
            """;
        using var stream = fakeTextToCopy.ToStream();
        var fakeBrowserFile = Substitute.For<IBrowserFile>();
        fakeBrowserFile.OpenReadStream(MaxFileSize).Returns(stream);
        var localizer = GetLocalizerMock();
        var sut = new ParseSeatsService(dbContextFactory, logger, seatsParser, appDataService, guidService, localizer);
        var result = await sut.CopyToTemporaryFileAsync(fakeBrowserFile);
        
        result.IsSuccess.Should().BeTrue();
        File.Exists("fc1fa5bb-b10c-481b-98ac-6f99a17cffcb.temp.csv").Should().BeTrue();
        File.ReadAllText("fc1fa5bb-b10c-481b-98ac-6f99a17cffcb.temp.csv").Should().Be(fakeTextToCopy);
        File.Delete("fc1fa5bb-b10c-481b-98ac-6f99a17cffcb.temp.csv");

    }

    [Fact]
    public async Task WhenErroOccuredWhileCopying_ThenReturnExcepiton()
    {
        var seatsParser = GetParserMock();
        var logger = GetLoggerMock();
        var dbContextFactory = GetDbContextFactoryMock();
        var appDataService = GetAppDataServiceMock();
        appDataService.GetTemporaryCsvFolderPath().Returns("");
        var guidService = GetGuidServiceMock();
        guidService.NewGuid().Returns(Guid.Parse("b122c68b-2b4b-434f-9c96-3493dfcdf309"));
        var fakeBrowserFile = Substitute.For<IBrowserFile>();
        fakeBrowserFile.OpenReadStream(MaxFileSize).Throws(new Exception("fake error"));
        var localizer = GetLocalizerMock();
        localizer["UnexpectedErrorCoping"].Returns(new LocalizedString("UnexpectedErrorCoping", "Unexpected error occured when coping seats"));
        var sut = new ParseSeatsService(dbContextFactory, logger, seatsParser, appDataService, guidService, localizer);
        var result = await sut.CopyToTemporaryFileAsync(fakeBrowserFile);
        result.ShouldBeFaultedWithMessage("Unexpected error occured when coping seats");
        logger.ReceivedLogError(new Exception("fake error"));
        File.Exists("fc1fa5bb-b10c-481b-98ac-6f99a17cffcb.temp.csv").Should().BeFalse();
    }


    private static SeatsParser GetParserMock() => Substitute.For<SeatsParser>(Substitute.For<ILogger<SeatsParser>>(), Substitute.For<IStringLocalizer<SeatsParser>>());
    private static ILogger<ParseSeatsService> GetLoggerMock() => Substitute.For<ILogger<ParseSeatsService>>();
    private static IDbContextFactory<CinemaDbContext> GetDbContextFactoryMock() => Substitute.For<IDbContextFactory<CinemaDbContext>>();
    private static AppDataService GetAppDataServiceMock() => Substitute.For<AppDataService>();
    private static GuidService GetGuidServiceMock() => Substitute.For<GuidService>();
    private static IStringLocalizer<ParseSeatsService> GetLocalizerMock() => Substitute.For<IStringLocalizer<ParseSeatsService>>();
}
