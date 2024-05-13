using CinemaBooking.Web.Exceptions;
using Result;
using System.Text.RegularExpressions;

namespace CinemaBooking.Web;

public static partial class SystemExtensions
{
    [GeneratedRegex(@"\s")]
    private static partial Regex SpaceRegex();
    public static IEnumerable<T> OrEmptyIfNull<T>(this IEnumerable<T>? source) =>
        source ?? []!;
    public static DateOnly ToDateOnly(this DateTime date) => DateOnly.FromDateTime(date);
    public static DateOnly ToDateOnly(this DateTimeOffset date) => DateOnly.FromDateTime(date.Date);
    public static T ReturnOrDoIfFailed<T>(this Result<T> result, Func<Exception, T> errorfunc)
    {
        return result.Match(x => x, errorfunc);
    }

    public static Task<Result<R>> MapResultAsync<A, R>(this Result<A> result, Func<A, Task<Result<R>>> func) =>
        result.Match(s => func(s), e => Task.FromResult(new Result<R>(e)));
    public static string RemoveSpaces(this string source) => SpaceRegex().Replace(source, "");

    public static void LogError(this ILogger logger, Exception? exception) => logger.LogError(exception, "");

    public static void LogErrorWithStackTrace(this ILogger logger, string message) => logger.LogError(new ExceptionWithStackTrace(message));
}
