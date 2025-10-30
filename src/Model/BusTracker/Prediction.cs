// Copyright (C) 2025 Micah Makaiwi

// This program is free software: you can redistribute it and/or modify it under the terms of the GNU Affero General
// Public License as published by the Free Software Foundation, version 3.
//
//This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied
//warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Affero General Public License for more
//details.
//
//You should have received a copy of the GNU Affero General Public License along with this program. If not, see
//<https://www.gnu.org/licenses/>.

using System.Text.Json.Serialization;

namespace MMKiwi.CtaTracker.Model.BusTracker;

public record Prediction(
    [property: JsonConverter(typeof(UnixToDateTimeConverter))]
    [property: JsonPropertyName("tmstmp")]
    DateTimeOffset Timestamp,
    [property: JsonPropertyName("typ")] string Type,
    [property: JsonPropertyName("stpnm")] string StopName,
    [property: JsonPropertyName("stpid")] StopId Stop,
    [property: JsonPropertyName("vid")] VehicleId VehicleId,
    [property: JsonPropertyName("dstp")] int DistanceToStop,
    [property: JsonPropertyName("rt")] RouteId Route,
    [property: JsonPropertyName("rtdd")] string RouteDesignator,
    [property: JsonPropertyName("rtdir")] string RouteDirection,
    [property: JsonPropertyName("des")] string Destination,
    [property: JsonConverter(typeof(UnixToDateTimeConverter))]
    [property: JsonPropertyName("prdtm")]
    DateTimeOffset PredictedTime,
    [property: JsonPropertyName("tablockid")]
    string TaBlockId,
    [property: JsonPropertyName("tatripid")]
    string TaTripId,
    [property: JsonPropertyName("origtatripno")]
    string OrigTaTripNo,
    [property: JsonPropertyName("dly")] bool IsDelayed,
    [property: JsonPropertyName("dyn")] int DynActionType,
    [property: JsonPropertyName("prdctdn")]
    string PredictionText,
    [property: JsonPropertyName("zone")] string Zone,
    [property: JsonPropertyName("psgld")] string PassengerCount,
    [property: JsonPropertyName("stst")] int ScheduledStart,
    [property: JsonPropertyName("stsd")] DateOnly ScheduledStartDate,
    [property: JsonPropertyName("flagstop")]
    FlagStop FlagStop
);