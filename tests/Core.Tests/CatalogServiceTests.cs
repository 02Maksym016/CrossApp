
using Core.Events;
using Core.Services;
using Core.Storage;

namespace Core.Tests;

public class CatalogServiceTests
{
    [Fact]
    public void Add_InvalidDuration_ThrowsAndDoesNotChangeState()
    {
        var store = new InMemoryCinemaStore();
        var service = new CinemaService(store);

        Assert.Throws<ArgumentOutOfRangeException>(() =>
            service.Add(
                "M-001",
                "Інтерстеллар",
                "Фантастика",
                2014,
                -10));

        Assert.Empty(service.All());
    }

    [Fact]
    public void Add_ValidMovie_RaisesChangedOnce()
    {
        var store = new InMemoryCinemaStore();
        var service = new CinemaService(store);

        var events = new List<CinemaChangedEventArgs>();

        service.Changed += (_, e) => events.Add(e);

        service.Add(
            "M-001",
            "Інтерстеллар",
            "Фантастика",
            2014,
            169);

        Assert.Single(events);
        Assert.Equal(CinemaChangeKind.Added, events[0].Kind);
        Assert.Single(service.All());
    }
}
