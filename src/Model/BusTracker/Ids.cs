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

using StronglyTypedIds;

namespace MMKiwi.CtaTracker.Model.BusTracker;

[StronglyTypedId(Template.String)]
public readonly partial struct VehicleId;

[StronglyTypedId(Template.String)]
public readonly partial struct RouteId;

[StronglyTypedId(Template.String)]
public readonly partial struct StopId;

[StronglyTypedId(Template.String)]
public readonly partial struct DirectionId;

[StronglyTypedId(Template.Guid)]
public readonly partial struct DetourId;

[StronglyTypedId(Template.Int)]
public readonly partial struct PatternId;