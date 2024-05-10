using CinemaBooking.Web.Db;
using CinemaBooking.Web.Services.Parsing;
using CinemaBooking.Web.UnitTests.TestHelpers;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace CinemaBooking.Web.UnitTests.Services.Parsing;
public class ParseSeatsService_CopyToTemporaryFileAsyncTest
{
    private const string SeatsFileName = "ParseSeatsService_CopyToTemporaryFileAsyncTest.temp.csv";
    private const long MaxFileSize = 1024 * 15;
    [Fact]
    public async Task WhenNoException_ThenCopyToTemporaryFile()
    {
        var seatsParser = GetParserMock();
        var logger = GetLoggerMock();
        var dbContextFactory = GetDbContextFactoryMock();
        var fakeTextToCopy = """
                1,2,3,4,5,6,7
                8,9,10
                11,12,13,14,15
            """;
        using var stream = GenerateStreamFromString(fakeTextToCopy);
        var fakeBrowserFile = Substitute.For<IBrowserFile>();
        fakeBrowserFile.OpenReadStream(MaxFileSize).Returns(stream);
        var sut = new ParseSeatsService(dbContextFactory, logger, seatsParser, GetParserSeatsServiceOptions());
        var result = await sut.CopyToTemporaryFileAsync(fakeBrowserFile);
        
        result.IsSuccess.Should().BeTrue();
        File.Exists(SeatsFileName).Should().BeTrue();
        File.ReadAllText(SeatsFileName).Should().Be(fakeTextToCopy);

    }

    [Fact]
    public async Task WhenErroOccuredWhileCopying_ThenReturnExcepiton()
    {
        var seatsParser = GetParserMock();
        var logger = GetLoggerMock();
        var dbContextFactory = GetDbContextFactoryMock();
        var fakeBrowserFile = Substitute.For<IBrowserFile>();
        fakeBrowserFile.OpenReadStream(MaxFileSize).Throws(new Exception("fake error"));
        var sut = new ParseSeatsService(dbContextFactory, logger, seatsParser, GetParserSeatsServiceOptions());

        var result = await sut.CopyToTemporaryFileAsync(fakeBrowserFile);
        result.ShouldBeFaultedWithMessage("Unexpected error occured when coping seats");
        logger.ReceivedLogError(new Exception("fake error"));
    }


    public static Stream GenerateStreamFromString(string @string)
    {
        var stream = new MemoryStream();
        var writer = new StreamWriter(stream);
        writer.Write(@string);
        writer.Flush();
        stream.Position = 0;
        return stream;
    }

    private static ParserSeatsServiceOptions GetParserSeatsServiceOptions() => new(SeatsFileName);
    private static SeatsParser GetParserMock() => Substitute.For<SeatsParser>(Substitute.For<ILogger<SeatsParser>>());
    private static ILogger<ParseSeatsService> GetLoggerMock() => Substitute.For<ILogger<ParseSeatsService>>();
    private static IDbContextFactory<CinemaDbContext> GetDbContextFactoryMock() => Substitute.For<IDbContextFactory<CinemaDbContext>>();
}
