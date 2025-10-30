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
using MMKiwi.CtaTracker.Model.BusTracker;

namespace MMKiwi.CtaTracker.Client;


[JsonSerializable(typeof(IReadOnlyList<Prediction>))]
[JsonSerializable(typeof(IReadOnlyList<Vehicle>))]
[JsonSerializable(typeof(IReadOnlyList<Detour>))]
[JsonSerializable(typeof(IReadOnlyList<Stop>))]
[JsonSerializable(typeof(IReadOnlyList<Pattern>))]
[JsonSerializable(typeof(IReadOnlyList<BusError>))]
[JsonSerializable(typeof(IReadOnlyList<Locale>))]
[JsonSerializable(typeof(IReadOnlyList<Route>))]
[JsonSerializable(typeof(IReadOnlyList<Direction>))]
[JsonSourceGenerationOptions(NumberHandling = JsonNumberHandling.AllowReadingFromString)]
public partial class BusSerializer: JsonSerializerContext;