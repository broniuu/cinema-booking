# Cinema Booking

Simple app to manage cinema screenings.

## Runing app

1. Run `dotnet run` or `dotnet watch run` command in `CinemaBooking.Web` directory.
1. Go to <https://localhost:7049/> in browser.

## Hall file rules

File with hall setting setting must have `.csv` extension.

Each row in file is equivalent of row in cinema hall.

Each cell in file is equivalent of seat in cinema hall:
- Number in cell is equivalent of seat number.
- "d" letter after seat number means, that this seat is reserved for people with disabillities. For instance 44d means that seats 44 is for people with disability.

Example file with hall settings is [here](/attachments/example.seed.csv)

![example seats file](/attachments/example_seats_file.png)

### Adding new hall setting

To add or change you need to:

1. go to **Add hall** panel.
1. Write name of hall.
1. Select delimiter of columns in csv file.
1. Select file with hall settings
1. Optionaly click "Show preview" button to review seats.
1. Click "Save hall" button.

> :warning: **When you save hall, all data related to old hall (also screenings, reservations) will be deleted permanently**: Be very careful here!

