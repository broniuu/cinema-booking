using Radzen;

namespace CinemaBooking.Web.Extensions;

public static class NotificationServiceExtensions
{
    public static void NotifyError(this NotificationService source, string message) =>
        source.Notify(new NotificationMessage { Severity = NotificationSeverity.Error, Detail = message});

    public static void NotifySuccess(this NotificationService source, string message) =>
        source.Notify(new NotificationMessage { Severity = NotificationSeverity.Success, Detail = message });
}
