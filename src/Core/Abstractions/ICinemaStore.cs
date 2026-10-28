
using Core.Domain;

namespace Core.Abstractions;

public interface ICinemaStore
{
    IReadOnlyList<Movie> List();

    Movie? GetById(string id);

    void Add(Movie movie);

    void Update(Movie movie);

    bool Remove(string id);
}
