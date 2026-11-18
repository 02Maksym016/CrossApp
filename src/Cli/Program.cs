
using Core;
using Core.Abstractions;
using Core.Services;
using Core.Storage;
using Core.Events;
using Core.Reports;
using Core.Domain;

bool useFile = args.Contains("--file");

string command = args.FirstOrDefault(
    arg => !arg.StartsWith("--")) ?? "report-top";

ICinemaStore store = useFile
    ? new FileCinemaStore(Path.Combine("data", "movies.json"))
    : new InMemoryCinemaStore(SampleData.CreateMovies());

CinemaService service = new(store);

if (command != "demo-events")
{
    switch (command)
    {
        case "report-top":
            Console.WriteLine("\nТОП-5 найдовших фільмів:");

            foreach (var movie in ReportService.TopByDuration(
                service.All(), 5))
            {
                Console.WriteLine(
                    $"{movie.Id} | {movie.Title} | {movie.Duration} хв | {movie.Genre}");
            }
            break;

        case "report-low":
            Console.WriteLine("\nФільми тривалістю менше 120 хвилин:");

            foreach (var movie in ReportService.ShortMovies(
                service.All(), 120))
            {
                Console.WriteLine(
                    $"{movie.Id} | {movie.Title} | {movie.Duration} хв | {movie.Genre}");
            }
            break;

        case "report-groups":
            Console.WriteLine("\nЗвіт за жанрами:");

            foreach (var group in ReportService.ByGenre(service.All()))
            {
                Console.WriteLine(
                    $"{group.Genre} | Кількість: {group.Count} | Загальна тривалість: {group.TotalDuration} хв");
            }
            break;

        case "report-sum":
            var totals = ReportService.Totals(service.All());

            Console.WriteLine("\nЗагальна статистика:");
            Console.WriteLine($"Кількість фільмів: {totals.Items}");
            Console.WriteLine($"Загальна тривалість: {totals.TotalDuration} хв");
            Console.WriteLine($"Середня тривалість: {totals.AverageDuration:F2} хв");
            Console.WriteLine($"Максимальна тривалість: {totals.MaxDuration} хв");
            break;

        case "report-deferred":
            var source = service.All().ToList();

            var query = source.Where(m => m.Duration < 120);

            Console.WriteLine("\nВідкладене виконання LINQ:");

            Console.WriteLine(
                $"До додавання фільму: {query.Count()}");

            source.Add(
                Movie.Create("M-999", "Тестовий фільм", "Тест", 2025, 90));

            Console.WriteLine(
                $"Після додавання фільму: {query.Count()}");

            var snapshot = query.ToList();

            source.Clear();

            Console.WriteLine(
                $"Кількість у збереженому знімку: {snapshot.Count}");

            break;

        default:
            Console.WriteLine("Невідома команда.");
            Console.WriteLine("Доступні команди: report-top, report-low, report-groups, report-sum, demo-events, report-deferred.");

            break;
    }

    return;
}

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