using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaBooking.Seed.Exceptions;
internal class SeatsParsingException : Exception
{
    public SeatsParsingException(string message) : base(message) { }
}
