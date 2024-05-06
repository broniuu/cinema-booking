using CinemaBooking.Seed.Dtos;
using CinemaBooking.Seed.Exceptions;
using CinemaBooking.Web.Dtos.HallPreview;
using Microsoft.VisualBasic.FileIO;

namespace CinemaBooking.Seed;
internal class SeatsParser
{
    private const string Delimiter = "\t";
    //Todo: Add README instruction
    internal static IEnumerable<SeatFromParsingDto> Parse(string csvFilePath)
    {
        using var parser = new TextFieldParser(csvFilePath);
        parser.TextFieldType = FieldType.Delimited;
        parser.SetDelimiters(Delimiter);
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
                    throw new SeatsParsingException($"field contains wrong data: \"{field}\"");
                }
                var seat = new SeatFromParsingDto()
                {
                    Id = Guid.NewGuid(),
                    PositionX = columnNumber,
                    PositionY = rowNumber,
                    SeatNumber = fieldValidatedForDisabled,
                    IsForDisabled = isForDisabled,
                };
                yield return seat;
                ++columnNumber;
            }
        }
    }

    internal static HallPreview ParseAsHallPreview(string csvFilePath)
    {
        using var parser = new TextFieldParser(csvFilePath);
        parser.TextFieldType = FieldType.Delimited;
        parser.SetDelimiters(Delimiter);
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
                    throw new SeatsParsingException($"field contains wrong data: \"{field}\"");
                }
                seatsInRow.Add(new SeatPreview(fieldValidatedForDisabled, isForDisabled));
            }
            rows.Add(new RowPreview(seatsInRow));
        }
        return new HallPreview(rows);
    }
}
