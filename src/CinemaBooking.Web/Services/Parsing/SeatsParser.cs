using CinemaBooking.Seed.Dtos;
using CinemaBooking.Seed.Exceptions;
using CinemaBooking.Web.Dtos.HallPreview;
using Result;
using Microsoft.VisualBasic.FileIO;
using Microsoft.Extensions.Localization;

namespace CinemaBooking.Web.Services.Parsing;
public class SeatsParser(ILogger<SeatsParser> logger, IStringLocalizer<SeatsParser> localizer)
{
    public static readonly Dictionary<string, string> AvailableDelimiters = new()
    {
        {"tab", "\t"},
        {",", ","},
        {";", ";"},
        {"space", " "},
    };
    private readonly ILogger<SeatsParser> _logger = logger;
    private readonly IStringLocalizer<SeatsParser> _localizer = localizer;

    public virtual Result<List<SeatFromParsingDto>?> Parse(string filePath, string delimiter)
    {
        if (!IsValidDelimiter(delimiter))
        {
            _logger.LogErrorWithStackTrace(_localizer["DelimiterNotValid"]);
            return new Result<List<SeatFromParsingDto>?>(new Exception(_localizer["DelimiterNotValid"]));
        }
        if (!File.Exists(filePath))
        {
            _logger.LogError("Temporary file in {Path} does not exists", filePath);
            return new Result<List<SeatFromParsingDto>?>(new Exception(_localizer["UnexpectedErrorParsing"]));
        }
        using var parser = new TextFieldParser(filePath);
        parser.TextFieldType = FieldType.Delimited;
        parser.SetDelimiters(delimiter);
        var seats = new List<SeatFromParsingDto>();
        for (var rowNumber = 1; !parser.EndOfData; ++rowNumber)
        {
            //Process row
            var fields = parser.ReadFields();
            var columnNumber = 1;
            foreach (var field in fields ?? Enumerable.Empty<string>())
            {
                if (field.Equals(""))
                {
                    ++columnNumber;
                    continue;
                }
                var isForDisabled = field.EndsWith('d');
                var fieldValidatedForDisabled = isForDisabled ? field[..^1] : field;
                if (!int.TryParse(fieldValidatedForDisabled, out _))
                {
                    _logger.LogErrorWithStackTrace($"field contains wrong data: \"{field}\"");
                    return new Result<List<SeatFromParsingDto>?>(new Exception(_localizer["NotValidData"]));
                }
                var seat = new SeatFromParsingDto()
                {
                    Id = Guid.NewGuid(),
                    PositionX = columnNumber,
                    PositionY = rowNumber,
                    SeatNumber = fieldValidatedForDisabled,
                    IsForDisabled = isForDisabled,
                };
                seats.Add(seat);
                ++columnNumber;
            }
        }
        return seats;
    }
    private static bool IsValidDelimiter(string delimiter) => AvailableDelimiters.ContainsValue(delimiter);

    public virtual Result<HallPreview?> ParseAsHallPreview(string filePath, string delimiter)
    {
        if (!IsValidDelimiter(delimiter))
        {
            _logger.LogErrorWithStackTrace("Passed delimiter is not valid.");
            return new Result<HallPreview?>(new Exception(_localizer["DelimiterNotValid"]));
        }
        if (!File.Exists(filePath))
        {
            _logger.LogError("Temporary file in {Path} does not exists", filePath);
            return new Result<HallPreview?>(new Exception(_localizer["UnexpectedErrorParsing"]));
        }
        using var parser = new TextFieldParser(filePath);
        parser.TextFieldType = FieldType.Delimited;
        parser.SetDelimiters(delimiter);
        var rows = new List<RowPreview>();
        for (var rowNumber = 1; !parser.EndOfData; ++rowNumber)
        {
            //Process row
            var fields = parser.ReadFields();
            var seatsInRow = new List<SeatPreview>();
            foreach (var field in fields ?? Enumerable.Empty<string>())
            {
                if (field.Equals(""))
                {
                    continue;
                }
                var isForDisabled = field.EndsWith('d');
                var fieldValidatedForDisabled = isForDisabled ? field[..^1] : field;
                if (!int.TryParse(fieldValidatedForDisabled, out _))
                {
                    _logger.LogErrorWithStackTrace($"field contains wrong data: \"{field}\"");
                    return new Result<HallPreview?>(new SeatsParsingException(_localizer["NotValidData"]));
                }
                seatsInRow.Add(new SeatPreview(fieldValidatedForDisabled, isForDisabled));
            }
            rows.Add(new RowPreview(seatsInRow));
        }
        return new HallPreview(rows);
    }
}
