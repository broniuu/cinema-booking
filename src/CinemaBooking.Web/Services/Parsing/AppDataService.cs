namespace CinemaBooking.Web.Services.Parsing;

public class AppDataService
{
    private const string AppDataFolderName = "CinemaBooking";
    private const string TemporaryCsvFolderName = "TempSeats";
    public virtual string GetAppDataPath() => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), AppDataFolderName);
    public virtual string GetTemporaryCsvFolderPath() => Path.Combine(GetAppDataPath(), TemporaryCsvFolderName);
    public virtual void CreateAppDataDirectories()
    {
        Directory.CreateDirectory(GetTemporaryCsvFolderPath());
    }
}
