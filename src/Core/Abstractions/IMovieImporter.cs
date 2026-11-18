
using Core.Domain;

namespace Core.Abstractions;

public interface IMovieImporter
{
    Task<IReadOnlyList<Movie>> LoadAsync(
        string path,
        CancellationToken cancellationToken = default);
}
