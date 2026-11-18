namespace Core.Reports;

public sealed record MovieReportLine(
    string Id,
    string Title,
    int Duration,
    string Genre);