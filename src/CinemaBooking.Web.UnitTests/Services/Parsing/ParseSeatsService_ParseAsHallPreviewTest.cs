using CinemaBooking.Web.Db;
using CinemaBooking.Web.Dtos.HallPreview;
using CinemaBooking.Web.Services;
using CinemaBooking.Web.Services.Parsing;
using CinemaBooking.Web.UnitTests.TestHelpers;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace CinemaBooking.Web.UnitTests.Services.Parsing;
public class ParseSeatsService_ParseAsHallPreviewTest
{
    private const long MaxFileSize = 1024 * 15;
    [Fact]
    public async Task WhenParsing_ReturnExpectedData()
    {
        var seatsParser = GetParserMock();
        seatsParser.ParseAsHallPreview(Arg.Any<string>(), Arg.Any<string>()).Returns(
            new HallPreview(
                [
                    new RowPreview([
                        new SeatPreview("1", false),
                        new SeatPreview("2", false),
                        new SeatPreview("3", false)
                        ]),
                    new RowPreview([
                        new SeatPreview("4", false),
                        new SeatPreview("5", false),
                        new SeatPreview("6", true),
                        new SeatPreview("7", false)
                        ])
                ]));
        var logger = GetLoggerMock();
        var dbContextFactory = GetDbContextFactoryMock();
        var appDataService = GetAppDataServiceMock();
        appDataService.GetTemporaryCsvFolderPath().Returns(string.Empty);
        var guidService = GetGuidServiceMock();
        guidService.NewGuid().Returns(Guid.Parse("790dfa24-d419-43cd-b0df-b61ab49dc025"));
        var localizer = GetLocalizerMock();
        var sut = new ParseSeatsService(dbContextFactory, logger, seatsParser, appDataService, guidService, localizer);
        var fakeBrowserFile = Substitute.For<IBrowserFile>();
        using var stream = string.Empty.ToStream();
        fakeBrowserFile.OpenReadStream(MaxFileSize).Returns(stream);
        await sut.CopyToTemporaryFileAsync(fakeBrowserFile);
        var result = sut.ParseAsHallPreview(",");
        result.GetOrDefault().Should().BeEquivalentTo(
            new
            {
                Rows = new object[]
                {
                    new
                    {
                        Seats = new object[]
                        {
                            new
                            {
                                Number = "1",
                                IsForDisabled = false
                            },
                            new
                            {
                                Number = "2",
                                IsForDisabled = false
                            },
                            new
                            {
                                Number = "3",
                                IsForDisabled = false
                            },
                        }
                    },
                    new
                    {
                        Seats = new object[]
                        {
                            new
                            {
                                Number = "4",
                                IsForDisabled = false,
                            },
                            new
                            {
                                Number = "5",
                                IsForDisabled = false,
                            },
                            new
                            {
                                Number = "6",
                                IsForDisabled = true,
                            },
                            new
                            {
                                Number = "7",
                                IsForDisabled = false,
                            },
                        }
                    }
                }
            }
            );
        seatsParser.Received().ParseAsHallPreview("790dfa24-d419-43cd-b0df-b61ab49dc025.temp.csv", ",");
    }
    private static AppDataService GetAppDataServiceMock() => Substitute.For<AppDataService>();
    private static GuidService GetGuidServiceMock() => Substitute.For<GuidService>();
    private static SeatsParser GetParserMock() => Substitute.For<SeatsParser>(Substitute.For<ILogger<SeatsParser>>(), Substitute.For<IStringLocalizer<SeatsParser>>());
    private static ILogger<ParseSeatsService> GetLoggerMock() => Substitute.For<ILogger<ParseSeatsService>>();
    private static IDbContextFactory<CinemaDbContext> GetDbContextFactoryMock() => Substitute.For<IDbContextFactory<CinemaDbContext>>();
    private static IStringLocalizer<ParseSeatsService> GetLocalizerMock() => Substitute.For<IStringLocalizer<ParseSeatsService>>();
}
