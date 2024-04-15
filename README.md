# Cinema Booking

Simple app to manage cinema screenings.

## Runing app

1. Run `dotnet run` or `dotnet watch run` command in `CinemaBooking.Web` directory.
1. Go to <https://localhost:7049/> in browser.

## Hall file rules

To seed data create `hall-seats-seed.csv` file in [Db](src/CinemaBooking.Web/Db) folder.

Each row in file is equivalent of row in cinema hall.

Each cell in file is equivalent of seat in cinema hall:
- Number in cell is equivalent of seat number.
- "d" letter after seat number means, that this seat is reserved for people with disabillities.