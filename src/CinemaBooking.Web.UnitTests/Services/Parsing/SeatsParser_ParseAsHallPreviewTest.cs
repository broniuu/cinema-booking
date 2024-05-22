using CinemaBooking.Web.Services.Parsing;
using CinemaBooking.Web.UnitTests.TestHelpers;
using Microsoft.Extensions.Logging;

namespace CinemaBooking.Web.UnitTests.Services.Parsing;
public sealed class SeatsParser_ParseAsHallPreviewTest : IDisposable
{
    private const string FakeSourceFileName = "SeatsParser_ParseAsHallPreviewTest.fake.csv";

    public void Dispose()
    {
        File.Delete(FakeSourceFileName);
    }

    [Fact]
    public void WhenFileIsValid_ThenReturnHallForView()
    {
        // Given
        using (var streamWriter = File.CreateText(FakeSourceFileName))
        {
            streamWriter.WriteLine("""
            1,2,3,4d,5,6,7,8,9,10
            11,12,13,14,15,16,17,18,19d
            """);
        }
        var logger = Substitute.For<ILogger<SeatsParser>>();
        var sut = new SeatsParser(logger);
        //When
        var result = sut.ParseAsHallPreview(FakeSourceFileName, ",");
        //Then
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
                                SeatNumber = "1",
                                IsForDisabled = false,
                            },
                            new
                            {
                                SeatNumber = "2",
                                IsForDisabled = false,
                            },
                            new
                            {
                                SeatNumber = "3",
                                IsForDisabled = false,
                            },
                            new
                            {
                                SeatNumber = "4",
                                IsForDisabled = true,
                            },
                            new
                            {
                                SeatNumber = "5",
                                IsForDisabled = false,
                            },
                            new
                            {
                                SeatNumber = "6",
                                IsForDisabled = false,
                            },
                            new
                            {
                                SeatNumber = "7",
                                IsForDisabled = false,
                            },
                            new
                            {
                                SeatNumber = "8",
                                IsForDisabled = false,
                            },
                            new
                            {
                                SeatNumber = "9",
                                IsForDisabled = false,
                            },
                            new
                            {
                                SeatNumber = "10",
                                IsForDisabled = false,
                            }
                        }
                    },
                    new
                    {
                        Seats = new object[]
                        {
                            new
                            {
                                SeatNumber = "11",
                                IsForDisabled = false,
                            },
                            new
                            {
                                SeatNumber = "12",
                                IsForDisabled = false,
                            },
                            new
                            {
                                SeatNumber = "13",
                                IsForDisabled = false,
                            },
                            new
                            {
                                SeatNumber = "14",
                                IsForDisabled = false,
                            },
                            new
                            {
                                SeatNumber = "15",
                                IsForDisabled = false,
                            },
                            new
                            {
                                SeatNumber = "16",
                                IsForDisabled = false,
                            },
                            new
                            {
                                SeatNumber = "17",
                                IsForDisabled = false,
                            },
                            new
                            {
                                SeatNumber = "18",
                                IsForDisabled = false,
                            },
                            new
                            {
                                SeatNumber = "19",
                                IsForDisabled = true,
                            }
                        }
                    }
                }
            }, o => o.ExcludingMissingMembers()); ;
    }

    [Theory]
    [InlineData(".")]
    [InlineData("/")]
    [InlineData("abc")]
    [InlineData("!")]
    public void WhenDelimiterIsNotValid_ThenReturnSeats(string wrongDelimiter)
    {
        // Given
        var logger = Substitute.For<ILogger<SeatsParser>>();
        var sut = new SeatsParser(logger);
        // When
        var result = sut.ParseAsHallPreview(FakeSourceFileName, wrongDelimiter);
        // Then
        result.ShouldBeFaultedWithMessage("Passed delimiter is not valid");
        logger.ReceivedLogErrorWithStackTrace("Passed delimiter is not valid");
    }

    [Theory]
    [InlineData(
    """
            1,2,3,4d,5,6,7,8,9,10
            11,12,13,14,15,16,17,18,19d
            """,
    " ",
    "1,2,3,4d,5,6,7,8,9,10")]
    [InlineData("""
            1,2,3e,4d
            11,12,13
            """,
    ",",
    "3e")]
    public void WhenDataInFileContainsNotValidData_ThenThrowExcepiton(string data, string delimiter, string failedField)
    {
        using (var streamWriter = File.CreateText(FakeSourceFileName))
        {
            streamWriter.WriteLine(data);
        }
        var logger = Substitute.For<ILogger<SeatsParser>>();
        var sut = new SeatsParser(logger);
        var result = sut.ParseAsHallPreview(FakeSourceFileName, delimiter);
        result.ShouldBeFaultedWithMessage("File contains not valid data");
        logger.ReceivedLogErrorWithStackTrace($"field contains wrong data: \"{failedField}\"");
    }
}
