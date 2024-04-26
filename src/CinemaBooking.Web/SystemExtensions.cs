using LanguageExt.Common;
using System.Globalization;
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
    public static string RemoveSpaces(this string source) => SpaceRegex().Replace(source, "");
}
