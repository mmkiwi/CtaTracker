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

using System.Runtime.InteropServices.JavaScript;

using Google.Protobuf;

using MMKiwi.CtaTracker.Model.BusTracker;
using MMKiwi.CtaTracker.Model.Protobuf;

using Riok.Mapperly.Abstractions;

namespace MMKiwi.CtaTracker.Model;

[Mapper]
public static partial class CtaMapper
{

    public static ErrorList ToProtobuf(this IEnumerable<BusError> errors)
    {
        var result = new ErrorList();
        result.Message.AddRange(errors.Select(x=>x.Message));
        return result;
    }

    public static long ToUnix(this DateTimeOffset dateTimeOffset)
    {
        return dateTimeOffset.ToUnixTimeSeconds();
    }

    public static DateTimeOffset ToDateTimeOffset(this long dateTimeOffset)
    {
        var ct = TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time");
        var result = DateTimeOffset.FromUnixTimeSeconds(dateTimeOffset);
        return result.ToOffset(ct.GetUtcOffset(result));
    }

    public static long ToUnix(this DateOnly dateTimeOffset)
    {
        var ct = TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time");
        return new DateTimeOffset(dateTimeOffset, default, ct.GetUtcOffset(dateTimeOffset.ToDateTime(default)))
            .ToUnixTimeSeconds();
    }

    public static DateOnly ToDateOnly(this long dateTimeOffset)
    {
        var ct = TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time");
        var result = DateTimeOffset.FromUnixTimeSeconds(dateTimeOffset);
        return DateOnly.FromDateTime(result.ToOffset(ct.GetUtcOffset(result)).DateTime);
    }

    private static VehicleId ToVehicleId(string id) => new(id);
    private static string ToString(VehicleId id) => id.Value;

    private static RouteId ToRouteId(string id) => new(id);
    private static string ToString(RouteId id) => id.Value;

    private static StopId ToStopId(string id) => new(id);
    private static string ToString(StopId id) => id.Value;

    private static DirectionId ToDirectionId(string id) => new(id);
    private static string ToString(DirectionId id) => id.Value;

    private static ByteString ToByteString(DetourId id) => ByteString.CopyFrom(id.Value.ToByteArray());

    private static DetourId ToDetourId(ByteString? id)
    {
        ArgumentNullException.ThrowIfNull(id);
        return new DetourId(new Guid(id.Span));
    }

    private static PatternId ToPatternId(int id) => new(id);
    private static int ToString(PatternId id) => id.Value;
}