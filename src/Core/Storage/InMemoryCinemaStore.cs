
using Core.Abstractions;
using Core.Domain;

namespace Core.Storage;

public sealed class InMemoryCinemaStore : ICinemaStore
{
    private readonly Dictionary<string, Movie> _items =
        new(StringComparer.OrdinalIgnoreCase);

    public InMemoryCinemaStore(IEnumerable<Movie>? seed = null)
    {
        if (seed is null)
            return;

        foreach (Movie movie in seed)
            Add(movie);
    }

    public IReadOnlyList<Movie> List() =>
        _items.Values.ToList();

    public Movie? GetById(string id) =>
        _items.GetValueOrDefault(id);

    public void Add(Movie movie)
    {
        ArgumentNullException.ThrowIfNull(movie);

        if (!_items.TryAdd(movie.Id, movie))
            throw new InvalidOperationException(
                $"Фільм з ID {movie.Id} уже існує");
    }

    public void Update(Movie movie)
    {
        ArgumentNullException.ThrowIfNull(movie);
        _items[movie.Id] = movie;
    }

    public bool Remove(string id) =>
        _items.Remove(id);
}
