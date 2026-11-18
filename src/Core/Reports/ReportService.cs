using Core.Domain;

namespace Core.Reports;

public static class ReportService
{
    public static IReadOnlyList<MovieReportLine> ShortMovies(
        IEnumerable<Movie> source,
        int threshold) =>
        source
            .Where(m => m.Duration < threshold)
            .OrderBy(m => m.Duration)
            .ThenBy(m => m.Id, StringComparer.Ordinal)
            .Select(m => new MovieReportLine(
                m.Id,
                m.Title,
                m.Duration,
                m.Genre))
            .ToList();

    public static IReadOnlyList<MovieReportLine> TopByDuration(
        IEnumerable<Movie> source,
        int n) =>
        source
            .OrderByDescending(m => m.Duration)
            .ThenBy(m => m.Id, StringComparer.Ordinal)
            .Take(n)
            .Select(m => new MovieReportLine(
                m.Id,
                m.Title,
                m.Duration,
                m.Genre))
            .ToList();

    public static IReadOnlyList<GenreSummary> ByGenre(
        IEnumerable<Movie> source) =>
        source
            .GroupBy(m => m.Genre, StringComparer.OrdinalIgnoreCase)
            .Select(g => new GenreSummary(
                g.Key,
                g.Count(),
                g.Sum(m => m.Duration)))
            .OrderByDescending(s => s.TotalDuration)
            .ToList();

    public static (
        int Items,
        int TotalDuration,
        double AverageDuration,
        int MaxDuration) Totals(
        IEnumerable<Movie> source)
    {
        var list = source.ToList();

        return list.Count == 0
            ? (0, 0, 0d, 0)
            : (
                list.Count,
                list.Sum(m => m.Duration),
                list.Average(m => m.Duration),
                list.Max(m => m.Duration));
    }
    // LINQ-запити виконуються не в момент створення,
    // а під час перебору результату або виклику ToList(), Count() тощо.
    // Якщо джерело змінити до виконання запиту,
    // результат може змінитися.
    // ToList() створює окремий список, який зберігає отриманий результат.
}