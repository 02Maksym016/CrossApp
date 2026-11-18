
using System.Text.Json;
using Core.Abstractions;
using Core.Domain;
using Core.Dto;

namespace Core.Storage;

public sealed class JsonMovieImporter : IMovieImporter
{
    public async Task<IReadOnlyList<Movie>> LoadAsync(
        string path,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(path))
            throw new ArgumentException(
                "Шлях до файлу не може бути порожнім",
                nameof(path));

        await using FileStream stream = new(
            path,
            FileMode.Open,
            FileAccess.Read,
            FileShare.Read,
            4096,
            useAsync: true);

        List<MovieDto?>? items =
            await JsonSerializer.DeserializeAsync<List<MovieDto?>>(
                stream,
                cancellationToken: cancellationToken);

        if (items is null)
            throw new DomainException(
                "Файл не містить списку фільмів");

        List<Movie> movies = [];

        for (int i = 0; i < items.Count; i++)
        {
            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                MovieDto dto = items[i]
                    ?? throw new ArgumentException(
                        "Запис фільму не може бути порожнім");

                movies.Add(Movie.FromDto(dto));
            }
            catch (ArgumentException ex)
            {
                throw new DomainException(
                    $"Помилка в записі фільму №{i + 1}: {ex.Message}",
                    ex);
            }
        }

        return movies;
    }
}
