using Core.Dto;
using Core.Import;

string path = args.Length > 0
    ? args[0]
    : Path.Combine("data", "sample.csv");

if (!File.Exists(path))
{
    Console.WriteLine($"Файл не знайдено: {Path.GetFullPath(path)}");
    return 1;
}

ImportResult<MovieDto> result = MovieCsvImporter.Load(path);

Console.WriteLine($"Завантажено записів: {result.Items.Count}");
Console.WriteLine();

Console.WriteLine("Перші записи:");

foreach (MovieDto movie in result.Items.Take(5))
{
    Console.WriteLine(
        $" {movie.Id,-6} {movie.Title,-25} {movie.Genre,-15} {movie.Year,4} {movie.Duration,3} хв");
}

if (result.Errors.Count > 0)
{
    Console.WriteLine();
    Console.WriteLine($"Пропущено рядків: {result.Errors.Count}");

    foreach (string error in result.Errors)
    {
        Console.WriteLine($" ! {error}");
    }
}

return 0;