
using System.Text.Json;
using Core.Abstractions;
using Core.Domain;
using Core.Dto;

namespace Core.Storage;

public sealed class FileCinemaStore : ICinemaStore
{
    private readonly string _filePath;
    private Dictionary<string, Movie>? _cache;

    public FileCinemaStore(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentException(
                "Шлях до файлу не може бути порожнім",
                nameof(filePath));

        _filePath = Path.GetFullPath(filePath);
    }

    public IReadOnlyList<Movie> List()
    {
        EnsureLoaded();
        return _cache!.Values.ToList();
    }

    public Movie? GetById(string id)
    {
        EnsureLoaded();
        return _cache!.GetValueOrDefault(id);
    }

    public void Add(Movie movie)
    {
        ArgumentNullException.ThrowIfNull(movie);
        EnsureLoaded();

        if (!_cache!.TryAdd(movie.Id, movie))
            throw new InvalidOperationException(
                $"Фільм з ID {movie.Id} уже існує");

        Flush();
    }

    public void Update(Movie movie)
    {
        ArgumentNullException.ThrowIfNull(movie);
        EnsureLoaded();

        _cache![movie.Id] = movie;
        Flush();
    }

    public bool Remove(string id)
    {
        EnsureLoaded();

        if (!_cache!.Remove(id))
            return false;

        Flush();
        return true;
    }

    private void EnsureLoaded()
    {
        if (_cache is not null)
            return;

        _cache = new Dictionary<string, Movie>(
            StringComparer.OrdinalIgnoreCase);

        if (!File.Exists(_filePath))
            return;

        string json = File.ReadAllText(_filePath);

        List<MovieDto> items =
            JsonSerializer.Deserialize<List<MovieDto>>(json) ?? [];

        foreach (MovieDto dto in items)
        {
            Movie movie = Movie.FromDto(dto);
            _cache[movie.Id] = movie;
        }
    }

    private void Flush()
    {
        string? directory = Path.GetDirectoryName(_filePath);

        if (directory is not null)
            Directory.CreateDirectory(directory);

        List<MovieDto> items = _cache!.Values
            .Select(movie => movie.ToDto())
            .ToList();

        string json = JsonSerializer.Serialize(
            items,
            new JsonSerializerOptions
            {
                WriteIndented = true
            });

        File.WriteAllText(_filePath, json);
    }
}
