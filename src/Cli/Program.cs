
using Core;
using Core.Abstractions;
using Core.Services;
using Core.Storage;
using Core.Events;

bool useFile = args.Contains("--file");

ICinemaStore store = useFile
    ? new FileCinemaStore(Path.Combine("data", "movies.json"))
    : new InMemoryCinemaStore(SampleData.CreateMovies());

CinemaService service = new(store);

var logPath = Path.Combine(
    AppContext.BaseDirectory, "logs", "app.log");

Directory.CreateDirectory(Path.GetDirectoryName(logPath)!);

void OnCinemaChanged(object? sender, CinemaChangedEventArgs e)
{
    var line = $"{e.At:HH:mm:ss} {e.Kind,-8} {e.Id} — {e.Title}";

    Console.WriteLine(line);
    File.AppendAllText(
    logPath,
    line + Environment.NewLine,
    System.Text.Encoding.UTF8);
}

service.Changed += OnCinemaChanged;

int changes = 0;
service.Changed += (_, _) => changes++;

Console.WriteLine(
    $"Сховище: {(useFile ? "FileCinemaStore" : "InMemoryCinemaStore")}");

Console.WriteLine("\nУсі фільми:");

foreach (var movie in service.All())
    Console.WriteLine(movie);

Console.WriteLine("\nДодавання нового фільму:");

try
{
    service.Add(
        "M-019",
        "Дюна",
        "Фантастика",
        2021,
        155);

    Console.WriteLine("Фільм успішно додано.");
}
catch (Exception ex)
{
    Console.WriteLine($"Помилка: {ex.Message}");
}

Console.WriteLine("\nПошук фільму M-001:");

var foundMovie = service.Find("M-001");

Console.WriteLine(foundMovie ?? null);

Console.WriteLine("\nОновлення фільму M-001:");

try
{
    service.Update(
        "M-001",
        "Інтерстеллар — спеціальний показ",
        "Фантастика",
        2014,
        170);

    Console.WriteLine("Фільм успішно оновлено.");
}
catch (Exception ex)
{
    Console.WriteLine($"Помилка: {ex.Message}");
}

Console.WriteLine("\nАрхівування фільму M-002:");

try
{
    var movie = service.Find("M-002");

    if (movie is null)
        Console.WriteLine("Фільм не знайдено.");
    else if (movie.IsArchived)
        Console.WriteLine("Фільм уже перебуває в архіві.");
    else
    {
        service.Archive("M-002");
        Console.WriteLine("Фільм успішно архівовано.");
    }
}
catch (Exception ex)
{
    Console.WriteLine($"Помилка: {ex.Message}");
}

Console.WriteLine("\nПідсумковий список фільмів:");

foreach (var movie in service.All())
    Console.WriteLine(movie);


Console.WriteLine("\nКількість отриманих подій:");
Console.WriteLine($"Подій отримано: {changes}");

Console.WriteLine("\nВідписка від першого обробника:");
service.Changed -= OnCinemaChanged;

service.Update(
    "M-001",
    "Інтерстеллар — фінальна версія",
    "Фантастика",
    2014,
    171);

Console.WriteLine($"Подій отримано: {changes}");

Console.WriteLine("\nПеревірка помилкової операції:");

int changesBeforeError = changes;

try
{
    service.Update("M-999", "Невідомий фільм", "Драма", 2025, 120);
}
catch (Exception ex)
{
    Console.WriteLine($"Помилка: {ex.Message}");
}

Console.WriteLine($"Подій до помилки: {changesBeforeError}");
Console.WriteLine($"Подій після помилки: {changes}");

Console.WriteLine("\nФільми тривалістю більше 150 хвилин:");

var longMovies = service.FindAll(movie => movie.Duration > 150);

foreach (var movie in longMovies)
    Console.WriteLine(movie);