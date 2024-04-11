using Microsoft.EntityFrameworkCore;

namespace CinemaBooking.Web.Db;

public class CinemaDbContext : DbContext
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        string dbPathDirectoryPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "CinemaBooking");
        Directory.CreateDirectory(dbPathDirectoryPath);
        var dbPath = Path.Combine(dbPathDirectoryPath, "cinemaBookingData.sqlite");
        optionsBuilder.UseSqlite($"Data Source={dbPath};");
    }
}
