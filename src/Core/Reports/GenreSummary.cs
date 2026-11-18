namespace Core.Reports;

public sealed record GenreSummary(
    string Genre,
    int Count,
    int TotalDuration);