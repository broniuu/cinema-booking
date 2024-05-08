using CinemaBooking.Web.Exceptions;
using LanguageExt.Common;
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

    public static void ReceivedLogErrorWithStackTrace(this ILogger logger, string message)
    {
        logger.Received().Log(
            LogLevel.Error,
            Arg.Any<EventId>(),
            Arg.Any<object>(),
            Arg.Is<ExceptionWithStackTrace>(e => e.Message == message),
            Arg.Any<Func<object, Exception?, string>>());
    }

    public static void ShouldBeFaultedWithMessage<T>(this Result<T> source, string message)
    {
        source.IsFaulted.Should().BeTrue();
        source.IfFail(e => e.Message.Should().Be(message));
    }

    public static T? GetOrDefault<T>(this Result<T> source) =>
        source.Match(s => s, e => default(T));
}