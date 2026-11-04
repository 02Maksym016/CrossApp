
using Core.Abstractions;
using Core.Domain;
using Core.Events;

namespace Core.Services;

public sealed class CinemaService
{
    public event EventHandler<CinemaChangedEventArgs>? Changed;
    private readonly ICinemaStore _store;

    public CinemaService(ICinemaStore store)
    {
        ArgumentNullException.ThrowIfNull(store);
        _store = store;
    }

    public IReadOnlyList<Movie> All() =>
        _store.List();
    
    public IReadOnlyList<Movie> FindAll(Func<Movie, bool> predicate)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        return _store.List().Where(predicate).ToList();
    }

    public Movie? Find(string id) =>
        _store.GetById(id);

    public Movie Add(string id, string title, string genre, int year, int duration)
    {
        Movie movie = Movie.Create(id, title, genre, year, duration);
        _store.Add(movie);
        OnChanged(movie, CinemaChangeKind.Added);
        return movie;
    }

    public void Update(string id, string title, string genre, int year, int duration)
    {
        Movie movie = _store.GetById(id)
            ?? throw new InvalidOperationException($"Фільм {id} не знайдено");

        movie.UpdateDetails(title, genre, year, duration);
        _store.Update(movie);

        OnChanged(movie, CinemaChangeKind.Updated);
    }

    public void Archive(string id)
    {
        Movie movie = _store.GetById(id)
            ?? throw new InvalidOperationException($"Фільм {id} не знайдено");

        movie.Archive();
        _store.Update(movie);

        OnChanged(movie, CinemaChangeKind.Archived);
    }

    public void Restore(string id)
    {
        Movie movie = _store.GetById(id)
            ?? throw new InvalidOperationException($"Фільм {id} не знайдено");

        movie.Restore();
        _store.Update(movie);

        OnChanged(movie, CinemaChangeKind.Restored);
    }

    public bool Remove(string id)
    {
        Movie? movie = _store.GetById(id);

        if (movie is null)
            return false;

        if (movie.IsArchived)
            throw new InvalidOperationException(
                $"Архівний фільм {id} не можна видалити");

        bool removed = _store.Remove(id);

        if (removed)
            OnChanged(movie, CinemaChangeKind.Removed);

        return removed;
    }

    private void OnChanged(Movie movie, CinemaChangeKind kind)
    {
        Changed?.Invoke(
            this,
            new CinemaChangedEventArgs(movie.Id, kind, movie.Title));
    }
}
