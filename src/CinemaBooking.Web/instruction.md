# Hall file rules

File with hall setting must have `.csv` extension.

Each row in file is equivalent of row in cinema hall.

Each cell in file is equivalent of seat in cinema hall:

- Number in cell is equivalent of seat number.
- "d" letter after seat number means, that this seat is reserved for people with disabillities. For instance 44d means that seat 44 is for people with disability.

Example above:

![example seats file](./example_seats_file.png)

## Adding new hall setting

Only one cinema hall can exist in system.

To add or change hall, you need to:

1. go to **Add hall** panel.
1. Write name of hall.
1. Select delimiter of columns in csv file.
1. Select file with hall settings
1. Optionaly click "Show preview" button to review seats.
1. Click "Save hall" button.

> :warning: **When you save hall, all data related to old hall (also screenings, reservations) will be deleted permanently**: Be very careful here!

