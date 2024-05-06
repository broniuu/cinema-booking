namespace CinemaBooking.Web;

public static class Utilities
{
    public static string GetAppLocalDataFolderPath() => 
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "CinemaBooking");
}
