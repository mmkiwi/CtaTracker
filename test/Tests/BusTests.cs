using MMKiwi.CtaTracker.Client;
using MMKiwi.CtaTracker.Model.BusTracker;
using Microsoft.Extensions.Configuration;
using MMKiwi.CtaTracker.Model;

namespace MMKiwi.CtaTracker.Tests;

public class BusTests
{
    public BusTests()
    {
        var config = new ConfigurationBuilder()
            .AddUserSecrets<BusTests>()
            .Build();
        Key = config["BusTrackerKey"] ??
              throw new InvalidOperationException("You must provide a valid secret key BusTrackerKey in usersecrets");
    }

    public string Key { get; }

    [Test]
    public async Task GetPredictionByStop(CancellationToken cancellationToken)
    {
        using BusTrackerClient client = new(Key);
        var result =
            await client.GetPredictions([new StopId("9269"), new StopId("14637"), new StopId("17096"), new StopId("14620")], cancellationToken: cancellationToken);
        await Assert.That(result.TryGetResult(out var resultValue)).IsTrue();
        await Assert.That(resultValue).IsNotNull().And.HasCount().GreaterThan(0);
    }

    [Test]
    public async Task GetPredictionByStopAndRoute(CancellationToken cancellationToken)
    {
        using BusTrackerClient client = new(Key);
        var result = await client.GetPredictions([new StopId("9269"), new StopId("14637")],
            [new RouteId("77")], 5, TimeResolution.Seconds, cancellationToken);
        await Assert.That(result.TryGetResult(out var resultValue)).IsTrue();
        await Assert.That(resultValue).IsNotNull().And.HasCount().GreaterThan(0);
    }

    [Test]
    public async Task GetPredictionThrows(CancellationToken cancellationToken)
    {
        using BusTrackerClient client = new(Key);
        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
        {
            await client.GetPredictions([new StopId("DOES NOT EXIST")], cancellationToken: cancellationToken);
        });
    }


    [Test]
    public async Task GetPredictionByVehicle(CancellationToken cancellationToken)
    {
        using BusTrackerClient client = new(Key);
        var result1 = await client.GetVehicles([new RouteId("77")], cancellationToken);
        await Assert.That(result1.TryGetResult(out var result1Value)).IsTrue();
        var firstId = result1Value![0].Id;
        var result = await client.GetPredictions([firstId], cancellationToken: cancellationToken);
        await Assert.That(result.TryGetResult(out var resultValue)).IsTrue();
        await Assert.That(resultValue).IsNotNull().And.HasCount().GreaterThan(0);
    }

    [Test]
    public async Task GetTime(CancellationToken cancellationToken)
    {
        using BusTrackerClient client = new(Key);
        var result = await client.GetTime(cancellationToken);
        await Assert.That(result.TryGetResult(out var resultValue)).IsTrue();
        await Assert.That((resultValue!.Time - DateTimeOffset.Now)!.Duration()).IsLessThan(TimeSpan.FromSeconds(10));
    }

    [Test]
    public async Task GetVehiclesByRoute(CancellationToken cancellationToken)
    {
        using BusTrackerClient client = new(Key);
        var result = await client.GetVehicles([new RouteId("77")], cancellationToken);
        await Assert.That(result.TryGetResult(out var resultValue)).IsTrue();
        await Assert.That(resultValue).IsNotNull().And.HasCount().GreaterThan(0);
    }

    [Test]
    public async Task GetVehiclesById(CancellationToken cancellationToken)
    {
        using BusTrackerClient client = new(Key);
        var result1 = await client.GetVehicles([new RouteId("77")], cancellationToken);
        await Assert.That(result1.TryGetResult(out var result1Value)).IsTrue();
        var firstId = result1Value![0].Id;
        var result = await client.GetVehicles([firstId], cancellationToken);
        await Assert.That(result.TryGetResult(out var resultValue)).IsTrue();
        await Assert.That(resultValue).IsNotNull().And.HasCount().GreaterThan(0);
    }

    [Test]
    public async Task GetRoutes(CancellationToken cancellationToken)
    {
        using BusTrackerClient client = new(Key);
        var result = await client.GetRoutes(cancellationToken);
        await Assert.That(result.TryGetResult(out var resultValue)).IsTrue();
        await Assert.That(resultValue).IsNotNull().And.HasCount().GreaterThan(0);
    }


    [Test]
    public async Task GetDirections(CancellationToken cancellationToken)
    {
        using BusTrackerClient client = new(Key);
        var result = await client.GetDirections(new RouteId("77"), cancellationToken);
        await Assert.That(result.TryGetResult(out var resultValue)).IsTrue();
        await Assert.That(resultValue).IsNotNull().And.HasCount().GreaterThan(0);
    }
    
    [Test]
    public async Task GetStopByRoute(CancellationToken cancellationToken)
    {
        using BusTrackerClient client = new(Key);
        var result = await client.GetStops(new RouteId("77"), new DirectionId("Eastbound"), cancellationToken);
        await Assert.That(result.TryGetResult(out var resultValue)).IsTrue();
        await Assert.That(resultValue).IsNotNull().And.HasCount().GreaterThan(0);
    }
    
    [Test]
    public async Task GetStopByStopId(CancellationToken cancellationToken)
    {
        using BusTrackerClient client = new(Key);
        var result = await client.GetStops([new StopId("9269"), new StopId("14637")], cancellationToken);
        await Assert.That(result.TryGetResult(out var resultValue)).IsTrue();
        await Assert.That(resultValue).IsNotNull().And.HasCount().GreaterThan(0);
    }
    
    [Test]
    public async Task GetPatternByRoute(CancellationToken cancellationToken)
    {
        using BusTrackerClient client = new(Key);
        var result = await client.GetPatterns(new RouteId("77"), cancellationToken);
        await Assert.That(result.TryGetResult(out var resultValue)).IsTrue();
        await Assert.That(resultValue).IsNotNull().And.HasCount().GreaterThan(0);
    }
    
        
    [Test]
    public async Task GetDetoursAll(CancellationToken cancellationToken)
    {
        using BusTrackerClient client = new(Key);
        var result = await client.GetDetours(cancellationToken: cancellationToken);
        await Assert.That(result.TryGetResult(out IReadOnlyList<Detour>? _)).IsTrue();
        await Assert.That(result.Result).IsNotNull();
    }
    [Test]
    public async Task GetLocale(CancellationToken cancellationToken)
    {
        using BusTrackerClient client = new(Key);
        var result = await client.GetLocaleList(cancellationToken);

        await Assert.That(result.TryGetResult(out var resultValue)).IsTrue();
        await Assert.That(resultValue).IsNotNull().And.HasCount().GreaterThan(0);
        await Assert.That(resultValue!.Count(x => x.Code == "en")).IsGreaterThan(0);
    }
    
    [Test]
    public async Task GetPatternById(CancellationToken cancellationToken)
    {
        using BusTrackerClient client = new(Key);
        var result = await client.GetPatterns([new PatternId(954)], cancellationToken);
        await Assert.That(result.TryGetResult(out IReadOnlyList<Pattern>? _)).IsTrue();
        await Assert.That(result.Result).IsNotNull();
    }
}