
using Core.Domain;
using Core.Reports;

namespace Core.Tests;

public class ReportServiceTests
{
    private static List<Movie> CreateMovies() =>
    [
        Movie.Create("M-001", "Фільм А", "Драма", 2020, 90),
        Movie.Create("M-002", "Фільм Б", "Комедія", 2021, 120),
        Movie.Create("M-003", "Фільм В", "Фантастика", 2022, 150),
        Movie.Create("M-004", "Фільм Г", "Драма", 2023, 180)
    ];

    [Theory]
    [InlineData(100, 1)]
    [InlineData(130, 2)]
    [InlineData(200, 4)]
    public void ShortMovies_ReturnsExpectedCount(
        int threshold,
        int expectedCount)
    {
        var movies = CreateMovies();

        var result = ReportService.ShortMovies(
            movies, threshold);

        Assert.Equal(expectedCount, result.Count);
    }

    [Fact]
    public void TopByDuration_ReturnsMoviesInDeterministicOrder()
    {
        var movies = CreateMovies();

        var result = ReportService.TopByDuration(movies, 3);

        Assert.Equal(
            new[] { "M-004", "M-003", "M-002" },
            result.Select(m => m.Id));
    }
}
