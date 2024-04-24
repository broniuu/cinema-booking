using Microsoft.Extensions.Logging;

namespace CinemaBooking.Web.UnitTests.TestHelpers;
public static class TestExtensions
{
    public static void ReceivedLogError(this ILogger logger, string message)
    {
        logger.Received().Log(
            LogLevel.Error,
            Arg.Any<EventId>(),
            Arg.Is<object>(o => o.ToString() == message),
            null,
            Arg.Any<Func<object, Exception?, string>>());
    }

    public static void ReceivedLogError<TExcpetion>(this ILogger logger, string message) where TExcpetion : Exception
    {
        logger.Received().Log(
            LogLevel.Error,
            Arg.Any<EventId>(),
            Arg.Is<object>(o => o.ToString() == message),
            Arg.Any<TExcpetion>(),
            Arg.Any<Func<object, Exception?, string>>());
    }
}