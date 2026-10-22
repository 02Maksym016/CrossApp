using Core.Domain;
using Core.Dto;

Console.WriteLine("=== Сценарій 1: успіх ===");

Movie movie = Movie.Create(
    "M-001",
    "Інтерстеллар",
    "Фантастика",
    2014,
    169);

Console.WriteLine(movie);

movie.UpdateDetails(
    "Інтерстеллар — спеціальний показ",
    "Фантастика",
    2014,
    170);

Console.WriteLine($"Після зміни: {movie}");

MovieDto dto = movie.ToDto();
Movie restored = Movie.FromDto(dto);

Console.WriteLine($"Відновлено з DTO: {restored}");

movie.Archive();

Console.WriteLine($"Після архівації: {movie}");

MovieDto archivedDto = movie.ToDto();
Movie restoredArchived = Movie.FromDto(archivedDto);

Console.WriteLine($"Відновлено архівний фільм: {restoredArchived}");

Console.WriteLine();
Console.WriteLine("=== Сценарій 2: порушення інваріантів ===");

TryDo(
    "порожній ID",
    () => Movie.Create("", "Тест", "Драма", 2020, 120));

TryDo(
    "порожня назва",
    () => Movie.Create("M-002", " ", "Драма", 2020, 120));

TryDo(
    "недопустимий рік",
    () => Movie.Create("M-003", "Тест", "Драма", 1800, 120));

TryDo(
    "нульова тривалість",
    () => Movie.Create("M-004", "Тест", "Драма", 2020, 0));

TryDo(
    "повторне архівування",
    () => movie.Archive());

TryDo(
    "зміна архівного фільму",
    () => movie.UpdateDetails(
        "Нова назва",
        "Драма",
        2020,
        120));

Console.WriteLine();
Console.WriteLine($"Стан після відмов: {movie}");

static void TryDo(string title, Action action)
{
    try
    {
        action();

        Console.WriteLine(
            $" {title}: виняток НЕ спрацював — інваріант відсутній!");
    }
    catch (Exception ex)
    {
        Console.WriteLine(
            $" {title}: {ex.GetType().Name} — {ex.Message}");
    }
}