
using Core;
using Core.Services;
using Core.Storage;

namespace Core.Tests;

public class ImportTests
{
    [Fact]
    public async Task ImportAsync_PreCancelledToken_ThrowsOperationCanceledException()
    {
        var store = new InMemoryCinemaStore();
        var service = new CinemaService(store);
        var importer = new JsonMovieImporter();

        using var cts = new CancellationTokenSource();
        cts.Cancel();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(
            () => service.ImportAsync(
                importer,
                "nonexistent.json",
                cts.Token));

        Assert.Empty(service.All());
    }

    [Fact]
    public async Task LoadAsync_InvalidMovieData_ThrowsDomainException()
    {
        string path = Path.GetTempFileName();

        try
        {
            await File.WriteAllTextAsync(
                path,
                """
                [
                  {
                    "Id": "M-001",
                    "Title": "Тестовий фільм",
                    "Genre": "Драма",
                    "Year": 2020,
                    "Duration": -10,
                    "IsArchived": false
                  }
                ]
                """);

            var importer = new JsonMovieImporter();

            await Assert.ThrowsAsync<DomainException>(
                () => importer.LoadAsync(path));
        }
        finally
        {
            File.Delete(path);
        }
    }
}

