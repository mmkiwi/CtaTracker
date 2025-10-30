// Copyright (C) 2025 Micah Makaiwi
// 
// This program is free software: you can redistribute it and/or modify it under the terms of the GNU Affero General
// Public License as published by the Free Software Foundation, version 3.
// 
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY

using System.Net.Http.Headers;

using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;

using MMKiwi.CtaTracker.Model.Protobuf;

namespace MMKiwi.CtaTracker.Server;

static class Extensions
{
    extension(InvalidOperationException)
    {
        internal static InvalidOperationException BothResultsErrors => new InvalidOperationException("Both errors and result are null");
        internal static InvalidOperationException MoreThanExpected => new InvalidOperationException("Server returned more results than expected");
    }
    
    extension(Prediction.Types.PassengerCount)
    {
        internal static Prediction.Types.PassengerCount ParseFast(string s) =>
            s.ToUpperInvariant() switch
            {
                "FULL" => Prediction.Types.PassengerCount.Full,
                "HALFEMPTY" => Prediction.Types.PassengerCount.HalfEmpty,
                "HALF_EMPTY" => Prediction.Types.PassengerCount.HalfEmpty,
                "EMPTY" => Prediction.Types.PassengerCount.Empty,
                _ => Prediction.Types.PassengerCount.NotApplicable
            };
    }

    extension(HttpRequest request)
    {
        public PerferedOutputFormat PerferedOutputFormat
        {
            get
            {
                if (request.Query.TryGetValue("format", out StringValues format))
                {
                    if (format == "json")
                        return PerferedOutputFormat.Json;
                    if (format == "protobuf")
                        return PerferedOutputFormat.Protobuf;
                }
                else if (request.Headers.TryGetValue(HeaderNames.Accept, out StringValues accept))
                {
                    IEnumerable<MediaTypeWithQualityHeaderValue> accepted = accept.First()?.Split(',')
                                                                                .Select(MediaTypeWithQualityHeaderValue
                                                                                    .Parse)
                                                                                .OrderByDescending(mt =>
                                                                                    mt.Quality.GetValueOrDefault(1)) ??
                                                                            Enumerable
                                                                                .Empty<
                                                                                    MediaTypeWithQualityHeaderValue>();
                    foreach (var acceptedFormat in accepted)
                    {
                        if (acceptedFormat.MediaType?.EndsWith("protobuf") ?? false)
                            return PerferedOutputFormat.Protobuf;
                        else if (acceptedFormat.MediaType?.EndsWith("json") ?? false)
                            return PerferedOutputFormat.Json;
                    }
                }

                return PerferedOutputFormat.Unknown;
            }
        }
    }
}