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

using NodaTime;

namespace MMKiwi.CtaTracker.Model.BusTracker;

public record Detour(
    [property: JsonPropertyName("id")] DetourId Id,
    [property: JsonPropertyName("ver")] int Version,
    [property: JsonPropertyName("st")] int State,
    [property: JsonPropertyName("desc")] string Description,
    [property: JsonPropertyName("rtdirs")] IReadOnlyList<RouteDirection> RouteDirections,
    [property: JsonPropertyName("startdt")]
    [property:JsonConverter(typeof(UnixToZonedDateTimeConverter))]
    ZonedDateTime StartDate,
    [property:JsonConverter(typeof(UnixToZonedDateTimeConverter))]
    [property: JsonPropertyName("enddt")] ZonedDateTime EndDate
);

public record RouteDirection(
    [property: JsonPropertyName("rt")] RouteId Route,
    [property: JsonPropertyName("dir")] DirectionId Direction
);