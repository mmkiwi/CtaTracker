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

public record Pattern(
    [property: JsonPropertyName("pid")] PatternId Id,
    [property: JsonPropertyName("ln")] double Length,
    [property: JsonPropertyName("rtdir")] DirectionId Direction,
    [property: JsonPropertyName("pt")] IReadOnlyList<Pattern.Point> Points
)
{
    public record Point(
        [property: JsonPropertyName("seq")] int Sequence,
        [property: JsonPropertyName("lat")] double Latitude,
        [property: JsonPropertyName("lon")] double Longitude,
        [property: JsonPropertyName("typ")] string Type,
        [property: JsonPropertyName("pdist")] double PointDistance,
        [property: JsonPropertyName("stpid")] StopId StopId,
        [property: JsonPropertyName("stpnm")] string StopName
    );
}