using CinemaBooking.Seed.Dtos;
using CinemaBooking.Seed.Exceptions;
using CinemaBooking.Web.Db.Entitites;
using Microsoft.VisualBasic.FileIO;

namespace CinemaBooking.Seed;
internal class SeatsParser
{
    //Todo: Add README instruction
    internal static IEnumerable<SeatFromParsingDto> Parse(string csvFilePath)
    {
        using var parser = new TextFieldParser(csvFilePath);
        parser.TextFieldType = FieldType.Delimited;
        parser.SetDelimiters(";");
        for (var rowNumber = 1; !parser.EndOfData;  ++rowNumber)
        {
            //Process row
            var fields = parser.ReadFields();
            var columnNumber = 1;
            foreach (var field in fields ?? Enumerable.Empty<string>())
            {
                if (field.Equals("E"))
                {
                    ++columnNumber;
                    continue;
                }
                if (!int.TryParse(field, out _))
                {
                    throw new SeatsParsingException($"field contains wrong data: \"{field}\"");
                }
                var seat = new SeatFromParsingDto()
                {
                    Id = Guid.NewGuid(),
                    PositionX = columnNumber,
                    PositionY = rowNumber,
                    SeatNumber = field,
                };
                yield return seat;
                ++columnNumber;
            }
        }
    } 
}
