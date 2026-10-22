using Core.Dto;

namespace Core.Domain;

public sealed class Movie
{
    private string _title;
    private string _genre;
    private int _year;
    private int _duration;
    private bool _isArchived;

    public string Id { get; }
    public string Title => _title;
    public string Genre => _genre;
    public int Year => _year;
    public int Duration => _duration;
    public bool IsArchived => _isArchived;

    private Movie(
        string id,
        string title,
        string genre,
        int year,
        int duration)
    {
        Id = id;
        _title = title;
        _genre = genre;
        _year = year;
        _duration = duration;
        _isArchived = false;
    }

    public static Movie Create(
        string id,
        string title,
        string genre,
        int year,
        int duration)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException(
                "Ідентифікатор фільму не може бути порожнім",
                nameof(id));

        ValidateDetails(title, genre, year, duration);

        return new Movie(
            id.Trim(),
            title.Trim(),
            genre.Trim(),
            year,
            duration);
    }

    public void UpdateDetails(
        string title,
        string genre,
        int year,
        int duration)
    {
        if (_isArchived)
            throw new InvalidOperationException(
                $"Архівний фільм {Id} не можна змінювати");

        ValidateDetails(title, genre, year, duration);

        _title = title.Trim();
        _genre = genre.Trim();
        _year = year;
        _duration = duration;
    }

    public void Archive()
    {
        if (_isArchived)
            throw new InvalidOperationException(
                $"Фільм {Id} уже перебуває в архіві");

        _isArchived = true;
    }

    public void Restore()
    {
        if (!_isArchived)
            throw new InvalidOperationException(
                $"Фільм {Id} не перебуває в архіві");

        _isArchived = false;
    }

    public MovieDto ToDto() => new(
        Id,
        Title,
        Genre,
        Year,
        Duration,
        IsArchived);

    public static Movie FromDto(MovieDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        Movie movie = Create(
            dto.Id,
            dto.Title,
            dto.Genre,
            dto.Year,
            dto.Duration);

        if (dto.IsArchived)
            movie.Archive();

        return movie;
    }

    private static void ValidateDetails(
        string title,
        string genre,
        int year,
        int duration)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException(
                "Назва фільму не може бути порожньою",
                nameof(title));

        if (string.IsNullOrWhiteSpace(genre))
            throw new ArgumentException(
                "Жанр фільму не може бути порожнім",
                nameof(genre));

        if (year < 1895 || year > DateTime.Now.Year)
            throw new ArgumentOutOfRangeException(
                nameof(year),
                year,
                $"Рік має бути в межах від 1895 до {DateTime.Now.Year}");

        if (duration <= 0)
            throw new ArgumentOutOfRangeException(
                nameof(duration),
                duration,
                "Тривалість фільму має бути більшою за нуль");
    }

    public override string ToString() =>
        $"{Id} — {Title} ({Year}), {Genre}, {Duration} хв, " +
        $"архів: {(IsArchived ? "так" : "ні")}";
}