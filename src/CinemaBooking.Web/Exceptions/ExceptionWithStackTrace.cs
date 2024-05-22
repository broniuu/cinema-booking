using System.Diagnostics;

namespace CinemaBooking.Web.Exceptions;

public class ExceptionWithStackTrace : Exception
{
    readonly string _stackTrace = new StackTrace(1, true).ToString();
    public ExceptionWithStackTrace()
    {
    }

    public ExceptionWithStackTrace(string message) : base(message)
    {
    }

    public override string? StackTrace => _stackTrace;
}
