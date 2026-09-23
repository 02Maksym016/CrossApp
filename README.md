# CrossApp

Наскрізний проєкт з крос-платформного програмування.

## Предметна область

Предметна область: Кінотеатр.

Сутності:
- Movie
- Hall
- Seat
- Ticket

Призначення: облік фільмів, кінозалів, місць та продажу квитків.

## Запуск

```bash
dotnet build
dotnet run --project src/Cli

## Структура solution

- `src/Core` — бібліотека зі спільною логікою отримання інформації про середовище.
- `src/Cli` — консольний застосунок для виводу інформації.
- `Cli` має посилання на `Core`.

Залежність проєктів:

`Cli → Core`

## Публікація

### Self-contained

```bash
dotnet publish src/Cli -c Release -r win-x64 --self-contained true -o publish/self-contained