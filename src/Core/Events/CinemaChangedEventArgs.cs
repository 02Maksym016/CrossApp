namespace Core.Events;

public sealed class CinemaChangedEventArgs(
    string id,
    CinemaChangeKind kind,
    string title) : EventArgs
{
    public string Id { get; } = id;

    public CinemaChangeKind Kind { get; } = kind;

    public string Title { get; } = title;

    public DateTimeOffset At { get; } = DateTimeOffset.Now;
}