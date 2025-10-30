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
public record Vehicle(
    
    [property: JsonPropertyName("vid")]
    VehicleId Id,
    [property: JsonConverter(typeof(UnixToDateTimeConverter))]
    [property: JsonPropertyName("tmstmp")] DateTimeOffset LastUpdated,
    [property: JsonPropertyName("lat")] double Latitude,
    [property: JsonPropertyName("lon")] double Longitude,
    [property: JsonPropertyName("hdg")] double Heading,
    [property: JsonPropertyName("pid")] PatternId Pattern,
    [property: JsonPropertyName("pdist")] double PatternDist,
    [property: JsonPropertyName("rt")] RouteId Route,
    [property: JsonPropertyName("des")] string Destination,
    [property: JsonPropertyName("dly")] bool IsDelayed,
    [property: JsonPropertyName("spdspd")] int Speed,
    [property: JsonPropertyName("tatripid")]
    string TaTripId,
    [property: JsonPropertyName("tablockid")]
    string TaBlockId,
    [property: JsonPropertyName("origtatripno")]
    string OriginalTaTripId,
    [property: JsonPropertyName("zone")] string Zone,
    [property: JsonPropertyName("mode")] int Mode,
    [property: JsonPropertyName("psgld")] string PassengerLoad,
    [property: JsonPropertyName("stst")] int ScheduledStartTime,
    [property: JsonPropertyName("stsd")] DateOnly ScheduledStartDate
);