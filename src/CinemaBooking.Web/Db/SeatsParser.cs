using CinemaBooking.Seed.Dtos;
using CinemaBooking.Seed.Exceptions;
using CinemaBooking.Web;
using CinemaBooking.Web.Dtos.HallPreview;
using LanguageExt.Common;
using Microsoft.VisualBasic.FileIO;

namespace CinemaBooking.Seed;
public class SeatsParser(ILogger<SeatsParser> logger)
{
    public static readonly Dictionary<string, string> AvailableDelimiters = new()
    {
        {"tab", "\t"},
        {",", ","},
        {";", ";"},
        {"space", " "},
    };
    private readonly ILogger<SeatsParser> _logger = logger;

    public Result<List<SeatFromParsingDto>?> Parse(string filePath, string delimiter)
    {
        if (!IsValidDelimiter(delimiter))
        {
            _logger.LogErrorWithStackTrace("Passed delimiter is not valid");
            return new Result<List<SeatFromParsingDto>?>(new Exception("Passed delimiter is not valid"));
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
                    _logger.LogError("field contains wrong data: \"{Field}\"", field);
                    return new Result<List<SeatFromParsingDto>?>(new Exception("File contains not valid data"));
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

    public Result<HallPreview?> ParseAsHallPreview(string csvFilePath, string delimiter)
    {
        if (!IsValidDelimiter(delimiter))
        {
            _logger.LogErrorWithStackTrace("Passed delimiter is not valid");
            return new Result<HallPreview?>(new Exception("Passed delimiter is not valid"));
        }
        using var parser = new TextFieldParser(csvFilePath);
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
                    return new Result<HallPreview?>(new SeatsParsingException("File contains not valid data"));
                }
                seatsInRow.Add(new SeatPreview(fieldValidatedForDisabled, isForDisabled));
            }
            rows.Add(new RowPreview(seatsInRow));
        }
        return new HallPreview(rows);
    }
}
