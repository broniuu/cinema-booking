using CinemaBooking.Web;
using CinemaBooking.Web.Db;
using CinemaBooking.Web.Db.Entitites;
using CinemaBooking.Web.Dtos.HallPreview;
using CinemaBooking.Web.Mappers;
using LanguageExt.Common;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.EntityFrameworkCore;

namespace CinemaBooking.Web.Services.Parsing;

public sealed class ParseSeatsService(IDbContextFactory<CinemaDbContext> dbContextFactory, ILogger<ParseSeatsService> logger, SeatsParser seatsParser, ParserSeatsServiceOptions parseSeatsServiceOptions) : IDisposable
{
    private readonly IDbContextFactory<CinemaDbContext> _dbContextFactory = dbContextFactory;
    private readonly ILogger<ParseSeatsService> _logger = logger;
    private readonly SeatsParser _seatsParser = seatsParser;
    private readonly ParserSeatsServiceOptions _parseSeatsServiceOptions = parseSeatsServiceOptions;
    private string TemporaryHallFilePath => _parseSeatsServiceOptions.TemporaryFilePath;
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
            _logger.LogError(ex);
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
            await using var dbContext = await _dbContextFactory.CreateDbContextAsync();
            await dbContext.Database.ExecuteSqlAsync($"TRUNCATE TABLE {nameof(CinemaDbContext.Halls)}");
            var hall = new Hall() { Name = hallName, Id = Guid.NewGuid() };
            await dbContext.Halls.AddAsync(hall);
            await dbContext.Database.ExecuteSqlAsync($"TRUNCATE TABLE {nameof(CinemaDbContext.Seats)}");
            var seats = seatsFromParsing!.Select(s => s.ToEntity(hall));
            await dbContext.Seats.AddRangeAsync(seats);
            await dbContext.SaveChangesAsync();
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
