﻿using CinemaBooking.Web.Exceptions;
using Result;
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


    public static void ReceivedLogError<TException>(this ILogger logger, TException exception) where TException : Exception
    {
        logger.Received().Log(
            LogLevel.Error,
            Arg.Any<EventId>(),
            Arg.Any<object>(),
            Arg.Is<TException>(e => e.Message == exception.Message),
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

    public static Stream ToStream(this string text)
    {
        var stream = new MemoryStream();
        var writer = new StreamWriter(stream);
        writer.Write(text);
        writer.Flush();
        stream.Position = 0;
        return stream;
    }
}