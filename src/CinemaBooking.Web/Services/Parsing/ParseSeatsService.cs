using CinemaBooking.Web;
using CinemaBooking.Web.Db;
using CinemaBooking.Web.Db.Entitites;
using CinemaBooking.Web.Dtos.HallPreview;
using CinemaBooking.Web.Mappers;
using LanguageExt.Common;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.EntityFrameworkCore;

namespace CinemaBooking.Web.Services.Parsing;

public sealed class ParseSeatsService(CinemaDbContext dbContext, ILogger<ParseSeatsService> logger, SeatsParser seatsParser) : IDisposable
{
    private readonly CinemaDbContext _dbContext = dbContext;
    private readonly ILogger<ParseSeatsService> _logger = logger;
    private readonly SeatsParser _seatsParser = seatsParser;

    private const string SeatsTemporaryFileName = "hall-seats.temp.csv";
    private static string TemporaryHallFilePath => Path.Combine(
        Utilities.GetAppLocalDataFolderPath(),
        SeatsTemporaryFileName);
    private const long MaxFileSize = 1024 * 15;

    public Result<HallPreview?> ParseAsHallPreview(string delimiter)
    {
        return _seatsParser.ParseAsHallPreview(TemporaryHallFilePath, delimiter);
    }

    public async Task<Result<bool>> CopyToTemporaryFileAsync(IBrowserFile file)
    {
        try
        {
            await using FileStream fs = new(TemporaryHallFilePath, FileMode.Create);
            await file.OpenReadStream(MaxFileSize).CopyToAsync(fs);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError("{Error}", ex);
            return new Result<bool>(new Exception("Unexpected error occured when coping seats"));
        }
    }

    public async Task<Result<bool>> SaveSeatsFromTempFileToDatabase(string hallName, string delimiter)
    {
        if (File.Exists(TemporaryHallFilePath))
        {
            _logger.LogError("Temporary file in {Path} does not exists", TemporaryHallFilePath);
            return new Result<bool>(new Exception("Unexpected error occured when saving seats"));
        }
        try
        {
            Exception? exception = null;
            var seatsFromParsing = _seatsParser.Parse(TemporaryHallFilePath, delimiter)
                .ReturnOrDoIfFailed(e =>
                {
                    exception = e;
                    return null;
                });
            if (exception is not null)
            {
                return new Result<bool>(new Exception("Unexpected error occured when saving seats"));
            }
            await _dbContext.Database.ExecuteSqlAsync($"TRUNCATE TABLE {nameof(CinemaDbContext.Halls)}");
            var hall = new Hall() { Name = hallName, Id = Guid.NewGuid() };
            await _dbContext.Halls.AddAsync(hall);
            await _dbContext.Database.ExecuteSqlAsync($"TRUNCATE TABLE {nameof(CinemaDbContext.Seats)}");
            var seats = seatsFromParsing!.Select(s => s.ToEntity(hall));
            await _dbContext.Seats.AddRangeAsync(seats);
            await _dbContext.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error occured when saving seats");
            return new Result<bool>(new Exception("Unexpected error occured when saving seats"));
        }
        return true;
    }

    public void DeleteTemporaryHallFile() => File.Delete(TemporaryHallFilePath);

    public void Dispose() => DeleteTemporaryHallFile();
}
