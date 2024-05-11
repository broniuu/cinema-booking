using CinemaBooking.Web.Db;
using CinemaBooking.Web.Db.Entitites;
using CinemaBooking.Web.Dtos.HallPreview;
using CinemaBooking.Web.Mappers;
using LanguageExt.Common;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.EntityFrameworkCore;

namespace CinemaBooking.Web.Services.Parsing;

public sealed class ParseSeatsService(
    IDbContextFactory<CinemaDbContext> dbContextFactory,
    ILogger<ParseSeatsService> logger,
    SeatsParser seatsParser,
    AppDataService appDataService,
    GuidService guidService) : IDisposable
{
    private readonly IDbContextFactory<CinemaDbContext> _dbContextFactory = dbContextFactory;
    private readonly ILogger<ParseSeatsService> _logger = logger;
    private readonly SeatsParser _seatsParser = seatsParser;
    private readonly AppDataService _appDataService = appDataService;
    private readonly GuidService _guidService = guidService;

    private string? _temporaryHallFilePath;
    private const long MaxFileSize = 1024 * 15;

    public Result<HallPreview?> ParseAsHallPreview(string delimiter)
    {
        if (_temporaryHallFilePath is null)
        {
            _logger.LogErrorWithStackTrace("Temporary file does not exists");
            return new Result<HallPreview?>(new Exception("Unexpected error occured when parsing seats"));
        }
        return _seatsParser.ParseAsHallPreview(_temporaryHallFilePath, delimiter);
    }

    private string CreateTempFilePath() => Path.Combine(_appDataService.GetTemporaryCsvFolderPath(), $"{_guidService.NewGuid()}.temp.csv");

    public async Task<Result<bool>> CopyToTemporaryFileAsync(IBrowserFile file)
    {
        _temporaryHallFilePath = CreateTempFilePath();
        try
        {
            await using FileStream fs = new(_temporaryHallFilePath, FileMode.Create);
            await file.OpenReadStream(MaxFileSize).CopyToAsync(fs);
            return true;
        }
        catch (Exception ex)
        {
            File.Delete(_temporaryHallFilePath);
            _temporaryHallFilePath = null;
            _logger.LogError(ex);
            return new Result<bool>(new Exception("Unexpected error occured when coping seats"));
        }
    }

    public async Task<Result<bool>> SaveSeatsFromTempFileToDbAsync(string hallName, string delimiter)
    {
        if (!File.Exists(_temporaryHallFilePath))
        {
            _logger.LogErrorWithStackTrace("Temporary file does not exists");
            return new Result<bool>(new Exception("Unexpected error occured when saving seats"));
        }
        return await _seatsParser.Parse(_temporaryHallFilePath, delimiter).MapResultAsync(async seatsFromParsing =>
        {
            try
            {
                await using var dbContext = await _dbContextFactory.CreateDbContextAsync();
                // Todo: If relational db will be implemented, add this code as transaction https://learn.microsoft.com/en-us/ef/core/saving/execute-insert-update-delete#transactions
                await dbContext.Halls.ExecuteDeleteAsync();
                var hall = new Hall() { Name = hallName, Id = _guidService.NewGuid() };
                await dbContext.Halls.AddAsync(hall);
                await dbContext.Seats.ExecuteDeleteAsync();
                var seats = seatsFromParsing!.Select(s => s.ToEntity(hall));
                await dbContext.Seats.AddRangeAsync(seats);
                await dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                return new Result<bool>(new Exception("Unexpected error occured when saving seats"));
            }
        });
    }

    public void DeleteTemporaryHallFile()
    {
        if (_temporaryHallFilePath is null)
        {
            return;
        }
        File.Delete(_temporaryHallFilePath);
        _temporaryHallFilePath = null;
    }

    public void Dispose() => DeleteTemporaryHallFile();
}
