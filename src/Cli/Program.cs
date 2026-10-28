
using Core;
using Core.Abstractions;
using Core.Services;
using Core.Storage;

bool useFile = args.Contains("--file");

ICinemaStore store = useFile
    ? new FileCinemaStore(Path.Combine("data", "movies.json"))
    : new InMemoryCinemaStore(SampleData.CreateMovies());

CinemaService service = new(store);

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
