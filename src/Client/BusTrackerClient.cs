using System.Collections.Specialized;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Nodes;
using MMKiwi.CtaTracker.Model;
using MMKiwi.CtaTracker.Model.BusTracker;

namespace MMKiwi.CtaTracker.Client;

public sealed class BusTrackerClient : IDisposable
{
    private readonly bool _ownsClient;

    public BusTrackerClient(string key)
    {
        Client = new HttpClient();
        Key = key;
        _ownsClient = true;
    }

    private string Key { get; }

    public BusTrackerClient(string key, HttpClient client, bool ownsClient = true)
    {
        Client = client;
        _ownsClient = ownsClient;
        Key = key;
    }

    public async Task<Response<ServerTime>> GetTime(CancellationToken cancellationToken = default)
    {
        const string baseUrl = "https://www.ctabustracker.com/bustime/api/v3/gettime";
        NameValueCollection queryString = System.Web.HttpUtility.ParseQueryString(string.Empty);

        queryString.Add("key", Key);
        queryString.Add("format", "json");
        queryString.Add("unixTime", "false");

        UriBuilder uriBuilder = new(baseUrl)
        {
            Query = queryString.ToString()
        };

        JsonObject? busTimeResponse = await GetAndUnroll(uriBuilder, cancellationToken);

        return busTimeResponse is null
            ? new Response<ServerTime>([new BusError("Response was null")])
            : new Response<ServerTime>(new ServerTime(UnixToZonedDateTimeConverter.ToZonedDateTime(busTimeResponse["tm"]!.GetValue<string>())));
    }

    public async Task<Response<IReadOnlyList<Vehicle>>> GetVehicles(IEnumerable<VehicleId> vehicles,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(vehicles);

        const string baseUrl = "https://www.ctabustracker.com/bustime/api/v3/getvehicles";
        NameValueCollection queryString = System.Web.HttpUtility.ParseQueryString(string.Empty);

        queryString.Add("key", Key);
        queryString.Add("format", "json");
        queryString.Add("unixTime", "true");
        queryString.Add("vid", string.Join(",", vehicles.Select(v => v.Value)));

        UriBuilder uriBuilder = new(baseUrl)
        {
            Query = queryString.ToString()
        };

        JsonObject? busTimeResponse = await GetAndUnroll(uriBuilder, cancellationToken);

        return busTimeResponse is null
            ? new Response<IReadOnlyList<Vehicle>>((IReadOnlyList<Vehicle>)[])
            : new Response<IReadOnlyList<Vehicle>>(busTimeResponse["vehicle"]
                                                       .Deserialize(BusSerializer.Default.IReadOnlyListVehicle) ??
                                                   throw new InvalidOperationException("Could not deserialize vehicle"));
    }

    public async Task<Response<IReadOnlyList<Stop>>> GetStops(RouteId route, DirectionId direction,
        CancellationToken cancellationToken = default)
    {
        const string baseUrl = "https://www.ctabustracker.com/bustime/api/v3/getstops";
        NameValueCollection queryString = System.Web.HttpUtility.ParseQueryString(string.Empty);

        queryString.Add("key", Key);
        queryString.Add("format", "json");
        queryString.Add("unixTime", "true");
        queryString.Add("rt", route.Value);
        queryString.Add("dir", direction.Value);

        UriBuilder uriBuilder = new(baseUrl)
        {
            Query = queryString.ToString()
        };

        JsonObject? busTimeResponse = await GetAndUnroll(uriBuilder, cancellationToken);
        return busTimeResponse is null
            ? new Response<IReadOnlyList<Stop>>((IReadOnlyList<Stop>)[])
            : new Response<IReadOnlyList<Stop>>(busTimeResponse["stops"]
                                                    .Deserialize(BusSerializer.Default.IReadOnlyListStop) ??
                                                throw new InvalidOperationException("Could not deserialize vehicle"));
    }

    public async Task<Response<IReadOnlyList<Stop>>> GetStops(IReadOnlyList<StopId> stops,
        CancellationToken cancellationToken = default)
    {
        const string baseUrl = "https://www.ctabustracker.com/bustime/api/v3/getstops";
        NameValueCollection queryString = System.Web.HttpUtility.ParseQueryString(string.Empty);

        queryString.Add("key", Key);
        queryString.Add("format", "json");
        queryString.Add("unixTime", "true");
        queryString.Add("stpid", string.Join(",", stops.Select(v => v.Value)));

        UriBuilder uriBuilder = new(baseUrl)
        {
            Query = queryString.ToString()
        };

        JsonObject? busTimeResponse = await GetAndUnroll(uriBuilder, cancellationToken);

        return busTimeResponse is null
            ? new Response<IReadOnlyList<Stop>>((IReadOnlyList<Stop>)[])
            : new Response<IReadOnlyList<Stop>>(busTimeResponse["stops"]
                                                    .Deserialize(BusSerializer.Default.IReadOnlyListStop) ??
                                                throw new InvalidOperationException("Could not deserialize vehicle"));
    }

    public async Task<Response<IReadOnlyList<Route>>> GetRoutes(CancellationToken cancellationToken = default)
    {
        const string baseUrl = "https://www.ctabustracker.com/bustime/api/v3/getroutes";
        NameValueCollection queryString = System.Web.HttpUtility.ParseQueryString(string.Empty);

        queryString.Add("key", Key);
        queryString.Add("format", "json");

        UriBuilder uriBuilder = new(baseUrl)
        {
            Query = queryString.ToString()
        };

        JsonObject? busTimeResponse = await GetAndUnroll(uriBuilder, cancellationToken);
        return busTimeResponse is null
            ? new Response<IReadOnlyList<Route>>((IReadOnlyList<Route>)[])
            : new Response<IReadOnlyList<Route>>(busTimeResponse["routes"]
                                                     .Deserialize(BusSerializer.Default.IReadOnlyListRoute) ??
                                                 throw new InvalidOperationException("Could not deserialize route"));
    }

    public async Task<Response<IReadOnlyList<Pattern>>> GetPatterns(RouteId route, CancellationToken cancellationToken = default)
    {
        const string baseUrl = "https://www.ctabustracker.com/bustime/api/v3/getpatterns";
        NameValueCollection queryString = System.Web.HttpUtility.ParseQueryString(string.Empty);

        queryString.Add("key", Key);
        queryString.Add("format", "json");
        queryString.Add("rt", route.Value);

        UriBuilder uriBuilder = new(baseUrl)
        {
            Query = queryString.ToString()
        };

        JsonObject? busTimeResponse = await GetAndUnroll(uriBuilder, cancellationToken);
        return busTimeResponse is null
            ? new Response<IReadOnlyList<Pattern>>((IReadOnlyList<Pattern>)[])
            : new Response<IReadOnlyList<Pattern>>(busTimeResponse["ptr"]
                                                       .Deserialize(BusSerializer.Default.IReadOnlyListPattern) ??
                                                   throw new InvalidOperationException("Could not deserialize pattern"));
    }

    public async Task<Response<IReadOnlyList<Locale>>> GetLocaleList(CancellationToken cancellationToken = default)
    {
        const string baseUrl = "https://www.ctabustracker.com/bustime/api/v3/getlocalelist";
        NameValueCollection queryString = System.Web.HttpUtility.ParseQueryString(string.Empty);

        queryString.Add("key", Key);
        queryString.Add("format", "json");

        UriBuilder uriBuilder = new(baseUrl)
        {
            Query = queryString.ToString()
        };

        JsonObject? busTimeResponse = await GetAndUnroll(uriBuilder, cancellationToken);
        return busTimeResponse is null
            ? new Response<IReadOnlyList<Locale>>((IReadOnlyList<Locale>)[])
            : new Response<IReadOnlyList<Locale>>(busTimeResponse["locale"]
                                                      .Deserialize(BusSerializer.Default.IReadOnlyListLocale) ??
                                                  throw new InvalidOperationException("Could not deserialize pattern"));
    }

    public async Task<Response<IReadOnlyList<Detour>>> GetDetours(RouteId? route = null, DirectionId? direction = null,
        CancellationToken cancellationToken = default)
    {
        if (route == null && direction != null)
            throw new InvalidOperationException("Cannot get detours for a direction without specifying a route");

        const string baseUrl = "https://www.ctabustracker.com/bustime/api/v3/getdetours";
        NameValueCollection queryString = System.Web.HttpUtility.ParseQueryString(string.Empty);

        queryString.Add("key", Key);
        queryString.Add("format", "json");
        if (route.HasValue)
            queryString.Add("rt", route.Value.ToString());
        if (direction.HasValue)
            queryString.Add("rtdir", direction.Value.ToString());

        UriBuilder uriBuilder = new(baseUrl)
        {
            Query = queryString.ToString()
        };

        JsonObject? busTimeResponse = await GetAndUnroll(uriBuilder, cancellationToken);
        return busTimeResponse is null
            ? new Response<IReadOnlyList<Detour>>((IReadOnlyList<Detour>)[])
            : new Response<IReadOnlyList<Detour>>(busTimeResponse["dtrs"]
                                                      .Deserialize(BusSerializer.Default.IReadOnlyListDetour) ??
                                                  throw new InvalidOperationException("Could not deserialize pattern"));
    }

    public async Task<Response<IReadOnlyList<Pattern>>> GetPatterns(IEnumerable<PatternId> patterns,
        CancellationToken cancellationToken = default)
    {
        const string baseUrl = "https://www.ctabustracker.com/bustime/api/v3/getpatterns";
        NameValueCollection queryString = System.Web.HttpUtility.ParseQueryString(string.Empty);

        queryString.Add("key", Key);
        queryString.Add("format", "json");
        queryString.Add("pid", string.Join(",", patterns.Select(v => v.Value)));

        UriBuilder uriBuilder = new(baseUrl)
        {
            Query = queryString.ToString()
        };

        JsonObject? busTimeResponse = await GetAndUnroll(uriBuilder, cancellationToken);
        return busTimeResponse is null
            ? new Response<IReadOnlyList<Pattern>>((IReadOnlyList<Pattern>)[])
            : new Response<IReadOnlyList<Pattern>>(busTimeResponse["ptr"]
                                                       .Deserialize(BusSerializer.Default.IReadOnlyListPattern) ??
                                                   throw new InvalidOperationException("Could not deserialize pattern"));
    }

    public async Task<Response<IReadOnlyList<Direction>>> GetDirections(RouteId route, CancellationToken cancellationToken = default)
    {
        const string baseUrl = "https://www.ctabustracker.com/bustime/api/v3/getdirections";
        NameValueCollection queryString = System.Web.HttpUtility.ParseQueryString(string.Empty);

        queryString.Add("key", Key);
        queryString.Add("format", "json");
        queryString.Add("rt", route.Value);

        UriBuilder uriBuilder = new(baseUrl)
        {
            Query = queryString.ToString()
        };

        JsonObject? busTimeResponse = await GetAndUnroll(uriBuilder, cancellationToken);
        return busTimeResponse is null
            ? new Response<IReadOnlyList<Direction>>((IReadOnlyList<Direction>)[])
            : new Response<IReadOnlyList<Direction>>(busTimeResponse["directions"]
                                                         .Deserialize(BusSerializer.Default.IReadOnlyListDirection) ??
                                                     throw new InvalidOperationException("Could not deserialize direction"));
    }

    public async Task<Response<IReadOnlyList<Vehicle>>> GetVehicles(IEnumerable<RouteId> routes,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(routes);

        const string baseUrl = "https://www.ctabustracker.com/bustime/api/v3/getvehicles";
        NameValueCollection queryString = System.Web.HttpUtility.ParseQueryString(string.Empty);

        queryString.Add("key", Key);
        queryString.Add("format", "json");
        queryString.Add("unixTime", "true");

        queryString.Add("rt", string.Join(",", routes.Select(v => v.Value)));

        UriBuilder uriBuilder = new(baseUrl)
        {
            Query = queryString.ToString()
        };

        JsonObject? busTimeResponse = await GetAndUnroll(uriBuilder, cancellationToken);
        return busTimeResponse is null
            ? new Response<IReadOnlyList<Vehicle>>((IReadOnlyList<Vehicle>)[])
            : new Response<IReadOnlyList<Vehicle>>(busTimeResponse["vehicle"]
                                                       .Deserialize(BusSerializer.Default.IReadOnlyListVehicle) ??
                                                   throw new InvalidOperationException("Could not deserialize vehicle"));
    }

    private async Task<JsonObject?> GetAndUnroll(UriBuilder uriBuilder, CancellationToken cancellationToken)
    {
        JsonNode? response;
#if DEBUG
        if (Debugger.IsAttached)
        {
            string responseStr = await Client.GetStringAsync(uriBuilder.Uri, cancellationToken);
            response = JsonNode.Parse(responseStr);
        }
        else
        {
#endif
            await using var stream = await Client.GetStreamAsync(uriBuilder.Uri, cancellationToken);
            response = await JsonNode.ParseAsync(stream, cancellationToken: cancellationToken);
#if DEBUG
        }
#endif

        if (response is not JsonObject responseObject ||
            !responseObject.ContainsKey("bustime-response") ||
            responseObject["bustime-response"] is not JsonObject busTimeResponse)
        {
            throw new InvalidOperationException("Could not deserialize response");
        }

        if (busTimeResponse.Count == 0) // null response
            return null;

        if (!busTimeResponse.ContainsKey("error"))
        {
            return busTimeResponse;
        }

        var errors = busTimeResponse["error"]
            .Deserialize(BusSerializer.Default.IReadOnlyListBusError) ?? [];
        throw new InvalidOperationException("Error from CTA service:\n" +
                                            string.Join("\n", errors.Select(e => e.Message)));


    }

    public async Task<Response<IReadOnlyList<Prediction>>> GetPredictions(IEnumerable<StopId> stops,
        IEnumerable<RouteId>? routes = null, int? top = null,
        TimeResolution? timeResolution = null, CancellationToken cancellationToken = default)
    {
        const string baseUrl = "https://www.ctabustracker.com/bustime/api/v3/getpredictions";
        NameValueCollection queryString = System.Web.HttpUtility.ParseQueryString(string.Empty);

        queryString.Add("key", Key);
        queryString.Add("format", "json");
        queryString.Add("stpid", string.Join(",", stops.Distinct()));
        queryString.Add("unixTime", "true");

        if (routes != null)
            queryString.Add("rt", string.Join(",", routes));

        if (top.HasValue)
            queryString.Add("top", top.ToString());

        if (timeResolution.HasValue)
            queryString.Add("timeResolution", timeResolution.Value.ToStringFast());

        UriBuilder uriBuilder = new(baseUrl)
        {
            Query = queryString.ToString()
        };

        JsonObject? busTimeResponse = await GetAndUnroll(uriBuilder, cancellationToken);
        return busTimeResponse is null
            ? new Response<IReadOnlyList<Prediction>>((IReadOnlyList<Prediction>)[])
            : new Response<IReadOnlyList<Prediction>>(busTimeResponse["prd"]
                                                          .Deserialize(BusSerializer.Default.IReadOnlyListPrediction) ??
                                                      throw new InvalidOperationException("Could not deserialize prediction"));

    }

    public async Task<Response<IReadOnlyList<Prediction>>> GetPredictions(IEnumerable<VehicleId> vehicles,
        int? top = null,
        TimeResolution? timeResolution = null, CancellationToken cancellationToken = default)
    {
        const string baseUrl = "https://www.ctabustracker.com/bustime/api/v3/getpredictions";
        NameValueCollection queryString = System.Web.HttpUtility.ParseQueryString(string.Empty);

        queryString.Add("key", Key);
        queryString.Add("format", "json");
        queryString.Add("vid", string.Join(",", vehicles));
        queryString.Add("unixTime", "true");

        if (top.HasValue)
            queryString.Add("top", top.ToString());

        if (timeResolution.HasValue)
            queryString.Add("timeResolution", timeResolution.Value.ToStringFast());

        UriBuilder uriBuilder = new(baseUrl)
        {
            Query = queryString.ToString()
        };

        JsonObject? busTimeResponse = await GetAndUnroll(uriBuilder, cancellationToken);
        return busTimeResponse is null
            ? new Response<IReadOnlyList<Prediction>>((IReadOnlyList<Prediction>)[])
            : new Response<IReadOnlyList<Prediction>>(busTimeResponse["prd"]
                                                          .Deserialize(BusSerializer.Default.IReadOnlyListPrediction) ??
                                                      throw new InvalidOperationException("Could not deserialize vehicle"));
    }

    private HttpClient Client { get; }

    public void Dispose()
    {
        if (_ownsClient)
            Client.Dispose();
    }
}