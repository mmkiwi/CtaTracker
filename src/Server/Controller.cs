// Copyright (C) 2025 Micah Makaiwi
// 
// This program is free software: you can redistribute it and/or modify it under the terms of the GNU Affero General
// Public License as published by the Free Software Foundation, version 3.
// 
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY

using Microsoft.Extensions.Caching.Memory;

using MMKiwi.CtaTracker.Client;
using MMKiwi.CtaTracker.Model;
using MMKiwi.CtaTracker.Model.BusTracker;
using MMKiwi.CtaTracker.Model.Protobuf;

using Protobuf = MMKiwi.CtaTracker.Model.Protobuf;
using BusTracker = MMKiwi.CtaTracker.Model.BusTracker;
using RouteList = MMKiwi.CtaTracker.Model.Protobuf.PredictionResponse.Types.RouteList;

namespace MMKiwi.CtaTracker.Server;

public class Controller : IDisposable
{
    public Controller(BusTrackerClient busClient, IMemoryCache memoryCache)
    {
        BusClient = busClient;
        MemoryCache = memoryCache;
    }

    private BusTrackerClient BusClient { get; }
    private IMemoryCache MemoryCache { get; }

    public void Dispose()
    {
        BusClient.Dispose();
    }

    public async Task<IResult> ServerTime(HttpRequest request, CancellationToken cancellationToken)
    {
        Response<ServerTime> ctaResponse = await BusClient.GetTime(cancellationToken);
        return request.PerferedOutputFormat switch
        {
            PerferedOutputFormat.Json => JsonResult.Create<TimeResponse.Types.Time, TimeResponse>(
                GetResponseAsync(ctaResponse)),
            PerferedOutputFormat.Protobuf => ProtobufResult.Create<TimeResponse.Types.Time, TimeResponse>(
                GetResponseAsync(ctaResponse)),
            _ => BadAccept.Instance
        };
    }

    public async Task<IResult> GetPredictions(HttpRequest request, string[] stopIds,
        string[] routeIds, CancellationToken cancellationToken)
    {
        IEnumerable<StopId> stops = stopIds.Select(stopId => new StopId(stopId));
        IEnumerable<RouteId> routes = routeIds.Select(stopId => new RouteId(stopId));
        Response<IReadOnlyList<BusTracker.Prediction>> ctaResponse =
            await BusClient.GetPredictions(stops, routes, cancellationToken: cancellationToken);
        return request.PerferedOutputFormat switch
        {
            PerferedOutputFormat.Json => JsonResult.Create<RouteList, PredictionResponse>(
                await GetResponseAsync(ctaResponse, cancellationToken)),
            PerferedOutputFormat.Protobuf => ProtobufResult.Create<RouteList, PredictionResponse>(
                await GetResponseAsync(ctaResponse, cancellationToken)),
            _ => BadAccept.Instance
        };
    }

    private async Task<PredictionResponse> GetResponseAsync(
        Response<IReadOnlyList<BusTracker.Prediction>> response, CancellationToken cancellationToken)
    {
        if (response is { Errors: not null })
        {
            return new PredictionResponse { Error = response.Errors.ToProtobuf() };
        }

        if (response is not { Result: not null })
        {
            throw InvalidOperationException.BothResultsErrors;
        }

        try
        {
            var result = new PredictionResponse() { Result = new() };
            var stops = result.Result.Stops;
            foreach (var responseResult in response.Result)
            {
                var (routeId, stopId, directionName, prediction) = GetResponse(responseResult);

                if (stops.FirstOrDefault(r => r.Id == stopId.Value) is not { } stop)
                {
                    var stopCache = await LoadStopFromCacheAsync(stopId, cancellationToken);
                    stop = new Protobuf.Stop()
                    {
                        Name = stopCache.Name,
                        Id = stopId.Value,
                        HasDetours = stopCache is { DetourAdd.Count: > 0 } or { DetourRemove.Count: > 0 },
                        Latitude = stopCache.Latitude,
                        Longitude = stopCache.Longitude,
                    };
                    stops.Add(stop);
                    
                }

                if (stop.Routes.FirstOrDefault(r => r.Id == routeId.Value) is not { } route)
                {
                    var routeCache = await LoadRouteFromCache(routeId, cancellationToken);
                    route = new Protobuf.Route()
                    {
                        Name = routeCache.Name,
                        Color = routeCache.Color,
                        Designation = routeCache.Designator,
                        Id = routeId.Value
                    };
                    stop.Routes.Add(route);
                }
                
                if (route.Directions.FirstOrDefault(d => d.Direction == directionName) is not { } direction)
                {
                    Protobuf.RouteDirection rd = new() { Direction = directionName };
                    route.Directions.Add(rd);
                    direction = rd;
                }

                direction.Predictions.Add(prediction);
            }

            return result;
        }
        catch (ControllerException ex)
        {
            return new PredictionResponse { Error = ex.Errors.ToProtobuf() };
        }
    }

    private (RouteId, StopId, string, Protobuf.Prediction) GetResponse(BusTracker.Prediction prediction) =>
        (prediction.Route, prediction.Stop, prediction.RouteDirection, new Protobuf.Prediction
        {
            IsDelayed = prediction.IsDelayed,
            PassengerCount = Protobuf.Prediction.Types.PassengerCount.ParseFast(prediction.PassengerCount),
            PredictedTime = prediction.PredictedTime.ToUnix(),
            PredictionText = prediction.PredictionText,
            Timestamp = prediction.Timestamp.ToUnix(),
            StopDistance = prediction.DistanceToStop,
            Destination = prediction.Destination
        });

    private async Task<BusTracker.Stop> LoadStopFromCacheAsync(StopId stopId, CancellationToken cancellationToken)
    {
        return await this.MemoryCache.GetOrCreateAsync($"Stop_{stopId}", async e =>
        {
            e.AbsoluteExpirationRelativeToNow = new TimeSpan(0, 30, 0);
            var stopResponse = await BusClient.GetStops([stopId], cancellationToken);

            if (stopResponse.Errors is not null)
                throw new ControllerException(stopResponse.Errors);
            if (stopResponse.Result is null or { Count: 0 })
                throw InvalidOperationException.BothResultsErrors;

            if (stopResponse.Result is { Count: not 1 })
                throw InvalidOperationException.MoreThanExpected;

            return stopResponse.Result[0];
        }) ?? throw new InvalidOperationException($"Error retrieving stop {stopId}");
    }

    private async Task<BusTracker.Route> LoadRouteFromCache(RouteId routeId, CancellationToken cancellationToken)
    {
        return await this.MemoryCache.GetOrCreateAsync($"Route_{routeId}", async e =>
        {
            e.AbsoluteExpirationRelativeToNow = new TimeSpan(0, 30, 0);
            var routeResponse = await BusClient.GetRoutes(cancellationToken);

            if (routeResponse.Errors is not null)
                throw new ControllerException(routeResponse.Errors);
            if (routeResponse.Result is null)
                throw InvalidOperationException.BothResultsErrors;

            return routeResponse.Result.Single(r => r.RouteId == routeId);
        }) ?? throw new InvalidOperationException($"Error retrieving route {routeId}");
    }

    private static TimeResponse GetResponseAsync(Response<ServerTime> response) =>
        response switch
        {
            { Errors: not null } => new TimeResponse { Error = response.Errors.ToProtobuf() },
            { Result: not null } => new TimeResponse { Result = new TimeResponse.Types.Time { Timestamp = response.Result.Time.ToUnix() } },
            _ => throw InvalidOperationException.BothResultsErrors
        };

    private class ControllerException : Exception
    {
        public ControllerException(IReadOnlyList<BusError> stopResponseErrors) : base(string.Join("\n",
            stopResponseErrors.Select(e => e.Message)))
        {
            Errors = stopResponseErrors;
        }

        public IReadOnlyList<BusError> Errors { get; }
    }
}