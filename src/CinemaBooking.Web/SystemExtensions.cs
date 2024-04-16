namespace CinemaBooking.Web;

public static class SystemExtensions
{
    public static IEnumerable<T> OrEmptyIfNull<T>(this IEnumerable<T>? source) =>
        source ?? []!;
}
