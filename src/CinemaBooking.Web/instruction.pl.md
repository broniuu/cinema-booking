# Zasady obs�ugi pliku z sal� kinow�

Plik z ustawieniem sali kinowej musi mie� rozszerzenie `.csv`.

Ka�dy wiersz w pliku jest odpowiednikiem rz�du w sali kinowej.

Ka�da kom�rka w pliku jest odpowiednikiem miejsca siedz�cego w sali kinowej:

- Numer w kom�rce jest odpowiednikiem numeru miejsca siedz�cego.
- Literka "d" po numerze miejsca siedz�cego oznacza, �e to miejsce jest zarezerwowane dla os�b z niepe�nosprawno�ci�. 

Na przyk�ad "44d" oznacza �e miejsce o numerze 44 jest przeznaczone dla os�b z niepe�nosprawno�ci�.

Przyk�ad pliku poni�ej:

![przyk�ad pliku z siedzeniami](./example_seats_file.png)

## Dodawanie nowej sali kinowej

W systemie mo�e istnie� tylko jedna sala kinowa.

Aby dodac lub usun�� sal�, nale�y:

1. Przej�� do okienka **Dodaj sal�**.
1. Wpisa� nazw� hali.
1. Wybra� separator kolumn u�yty w danym pliku csv.
1. Wybra� plik z ustawieniem sali.
1. Opcjonalnie mo�na klikn�� przycisk "Poka� podgl�d", aby zobaczy� jak b�dzie wygl�da�o ustawienie miejsc siedz�cych.
1. Klikn�� przycisk "Zapisz sal�".

> :warning: **Kiedy zapiszesz sal�, wszystkie dane powi�zane ze star� sal� (tak�e seanse i rezerwacj�) zostan� usuni�te na zawsze**: B�d� ostro�ny!