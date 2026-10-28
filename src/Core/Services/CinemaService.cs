
using Core.Abstractions;
using Core.Domain;

namespace Core.Services;

public sealed class CinemaService
{
    private readonly ICinemaStore _store;

    public CinemaService(ICinemaStore store)
    {
        ArgumentNullException.ThrowIfNull(store);
        _store = store;
    }

    public IReadOnlyList<Movie> All() =>
        _store.List();

    public Movie? Find(string id) =>
        _store.GetById(id);

    public Movie Add(
        string id,
        string title,
        string genre,
        int year,
        int duration)
    {
        Movie movie = Movie.Create(
            id,
            title,
            genre,
            year,
            duration);

        _store.Add(movie);
        return movie;
    }

    public void Update(
        string id,
        string title,
        string genre,
        int year,
        int duration)
    {
        Movie movie = _store.GetById(id)
            ?? throw new InvalidOperationException(
                $"Фільм {id} не знайдено");

        movie.UpdateDetails(title, genre, year, duration);
        _store.Update(movie);
    }

    public void Archive(string id)
    {
        Movie movie = _store.GetById(id)
            ?? throw new InvalidOperationException(
                $"Фільм {id} не знайдено");

        movie.Archive();
        _store.Update(movie);
    }

    public void Restore(string id)
    {
        Movie movie = _store.GetById(id)
            ?? throw new InvalidOperationException(
                $"Фільм {id} не знайдено");

        movie.Restore();
        _store.Update(movie);
    }

    public bool Remove(string id)
    {
        Movie? movie = _store.GetById(id);

        if (movie is null)
            return false;

        if (movie.IsArchived)
            throw new InvalidOperationException(
                $"Архівний фільм {id} не можна видалити");

        return _store.Remove(id);
    }
}
