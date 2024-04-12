using CinemaBooking.Seed;
using System.CommandLine;
using System.Text.Json;

Console.Title = "cinemaseed";

Console.WriteLine("hello world!");
var option = new Option<FileInfo>(
    ["--file", "-f"],
    "dasdsad");
var seats = SeatsParser.Parse(@"C:\Users\broni\source\repos\cinema-booking\src\CinemaBooking.Seed\hall-seats.csv");
Console.WriteLine(JsonSerializer.Serialize(seats));
