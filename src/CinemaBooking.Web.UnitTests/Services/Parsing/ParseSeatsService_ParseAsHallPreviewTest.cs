using CinemaBooking.Web.Db;
using CinemaBooking.Web.Dtos.HallPreview;
using CinemaBooking.Web.Services.Parsing;
using CinemaBooking.Web.UnitTests.TestHelpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CinemaBooking.Web.UnitTests.Services.Parsing;
public class ParseSeatsService_ParseAsHallPreviewTest
{
    private const string SeatsFileName = "ParseSeatsService_ParseAsHallPreviewTest.temp.csv";
    [Fact]
    public void WhenParsing_ReturnExpectedData()
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
        var sut = new ParseSeatsService(dbContextFactory, logger, seatsParser, GetParserSeatsServiceOptions());
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
        seatsParser.Received().ParseAsHallPreview(SeatsFileName, ",");
    }
    private static ParserSeatsServiceOptions GetParserSeatsServiceOptions() => new(SeatsFileName);
    private static SeatsParser GetParserMock() => Substitute.For<SeatsParser>(Substitute.For<ILogger<SeatsParser>>());
    private static ILogger<ParseSeatsService> GetLoggerMock() => Substitute.For<ILogger<ParseSeatsService>>();
    private static IDbContextFactory<CinemaDbContext> GetDbContextFactoryMock() => Substitute.For<IDbContextFactory<CinemaDbContext>>();
}
