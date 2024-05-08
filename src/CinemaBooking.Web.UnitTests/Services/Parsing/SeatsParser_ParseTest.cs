using CinemaBooking.Web.Services.Parsing;
using CinemaBooking.Web.UnitTests.TestHelpers;
using Microsoft.Extensions.Logging;

namespace CinemaBooking.Web.UnitTests.Services.Parsing;
public sealed class SeatsParser_ParseTest : IDisposable
{
    private const string FakeSourceFileName = "SeatsParser_ParseTest.fake.csv";

    public void Dispose()
    {
        File.Delete(FakeSourceFileName);
    }

    [Fact]
    public void WhenFileIsValid_ThenReturnSeats()
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
        var result = sut.Parse(FakeSourceFileName, ",");
        //Then
        result.GetOrDefault().Should().BeEquivalentTo(new object[]
            {
                new
                {
                    PositionX = 1,
                    PositionY = 1,
                    SeatNumber = "1",
                    IsForDisabled = false, 
                },
                new
                {
                    PositionX = 2,
                    PositionY = 1,
                    SeatNumber = "2",
                    IsForDisabled = false,
                },
                new
                {
                    PositionX = 3,
                    PositionY = 1,
                    SeatNumber = "3",
                    IsForDisabled = false,
                },
                new
                {
                    PositionX = 4,
                    PositionY = 1,
                    SeatNumber = "4",
                    IsForDisabled = true,
                },
                new
                {
                    PositionX = 5,
                    PositionY = 1,
                    SeatNumber = "5",
                    IsForDisabled = false,
                },
                new
                {
                    PositionX = 6,
                    PositionY = 1,
                    SeatNumber = "6",
                    IsForDisabled = false,
                },
                new
                {
                    PositionX = 7,
                    PositionY = 1,
                    SeatNumber = "7",
                    IsForDisabled = false,
                },
                new
                {
                    PositionX = 8,
                    PositionY = 1,
                    SeatNumber = "8",
                    IsForDisabled = false,
                },
                new
                {
                    PositionX = 9,
                    PositionY = 1,
                    SeatNumber = "9",
                    IsForDisabled = false,
                },
                new
                {
                    PositionX = 10,
                    PositionY = 1,
                    SeatNumber = "10",
                    IsForDisabled = false,
                },
                new
                {
                    PositionX = 1,
                    PositionY = 2,
                    SeatNumber = "11",
                    IsForDisabled = false,
                },
                new
                {
                    PositionX = 2,
                    PositionY = 2,
                    SeatNumber = "12",
                    IsForDisabled = false,
                },
                new
                {
                    PositionX = 3,
                    PositionY = 2,
                    SeatNumber = "13",
                    IsForDisabled = false,
                },
                new
                {
                    PositionX = 4,
                    PositionY = 2,
                    SeatNumber = "14",
                    IsForDisabled = false,
                },
                new
                {
                    PositionX = 5,
                    PositionY = 2,
                    SeatNumber = "15",
                    IsForDisabled = false,
                },
                new
                {
                    PositionX = 6,
                    PositionY = 2,
                    SeatNumber = "16",
                    IsForDisabled = false,
                },
                new
                {
                    PositionX = 7,
                    PositionY = 2,
                    SeatNumber = "17",
                    IsForDisabled = false,
                },
                new
                {
                    PositionX = 8,
                    PositionY = 2,
                    SeatNumber = "18",
                    IsForDisabled = false,
                },
                new
                {
                    PositionX = 9,
                    PositionY = 2,
                    SeatNumber = "19",
                    IsForDisabled = true,
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
        var result = sut.Parse(FakeSourceFileName, wrongDelimiter);
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
        var result = sut.Parse(FakeSourceFileName, delimiter);
        result.ShouldBeFaultedWithMessage("File contains not valid data");
        logger.ReceivedLogErrorWithStackTrace($"field contains wrong data: \"{failedField}\"");
    }
}
