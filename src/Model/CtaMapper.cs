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

using Google.Protobuf;

using MMKiwi.CtaTracker.Model.BusTracker;
using MMKiwi.CtaTracker.Model.Protobuf;

using Riok.Mapperly.Abstractions;

namespace MMKiwi.CtaTracker.Model;

[Mapper]
public static partial class CtaMapper
{
    public static partial BusTracker.Vehicle ToDto(this Protobuf.Vehicle vehicle);
    public static partial Protobuf.Vehicle ToProtobuf(this BusTracker.Vehicle vehicle);

    public static partial BusTracker.Stop ToDto(this Protobuf.Stop vehicle);
    public static partial Protobuf.Stop ToProtobuf(this BusTracker.Stop vehicle);

    public static partial BusTracker.Locale ToDto(this Protobuf.Locale vehicle);
    public static partial Protobuf.Locale ToProtobuf(this BusTracker.Locale vehicle);

    public static partial BusTracker.Detour ToDto(this Protobuf.Detour vehicle);
    public static partial Protobuf.Detour ToProtobuf(this BusTracker.Detour vehicle);

    public static partial BusTracker.Direction ToDto(this Protobuf.Direction vehicle);
    public static partial Protobuf.Direction ToProtobuf(this BusTracker.Direction vehicle);

    public static partial BusTracker.Pattern ToDto(this Protobuf.Pattern vehicle);
    public static partial Protobuf.Pattern ToProtobuf(this BusTracker.Pattern vehicle);

    public static partial BusTracker.Pattern.Point ToDto(this Protobuf.Pattern.Types.Point vehicle);
    public static partial Protobuf.Pattern.Types.Point ToProtobuf(this BusTracker.Pattern.Point vehicle);

    public static partial BusTracker.Prediction ToDto(this Protobuf.Prediction vehicle);
    public static partial Protobuf.Prediction ToProtobuf(this BusTracker.Prediction vehicle);

    public static partial BusTracker.Route ToDto(this Protobuf.Route vehicle);
    public static partial Protobuf.Route ToProtobuf(this BusTracker.Route vehicle);

    public static ErrorList ToProtobuf(this IReadOnlyList<BusError> error)
    {
        ErrorList output = new();
        output.Message.AddRange(error.Select(e => e.Message));
        return output;
    }

    public static RouteResponse ToProtobuf(this Response<IReadOnlyList<BusTracker.Route>> response)
    {
        if (response.TryGetResult(out var result))
        {
            var output = new RouteResponse
            {
                Result = new RouteResponse.Types.RouteList()
            };
            output.Result.Routes.AddRange(result.Select(r => r.ToProtobuf()));
            return output;
        }
        else if (response.TryGetErrors(out var errors))
        {
            return new RouteResponse
            {
                Error = errors.ToProtobuf()
            };
        }
        else
            throw new InvalidOperationException("One of result or errors must be set");
    }

    public static TimeResponse ToProtobuf(this Response<ServerTime> response)
    {
        if (response.TryGetResult(out var result))
        {
            return new TimeResponse
            {
                Result = result.Time.ToUnix()
            };
        }
        else if (response.TryGetErrors(out var errors))
        {
            return new TimeResponse
            {
                Error = errors.ToProtobuf()
            };
        }
        else throw new InvalidOperationException("One of result or errors must be set");
    }

    private static long ToUnix(this DateTimeOffset dateTimeOffset)
    {
        return dateTimeOffset.ToUnixTimeSeconds();
    }

    private static DateTimeOffset ToDateTimeOffset(this long dateTimeOffset)
    {
        var ct = TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time");
        var result = DateTimeOffset.FromUnixTimeSeconds(dateTimeOffset);
        return result.ToOffset(ct.GetUtcOffset(result));
    }

    private static long ToUnix(this DateOnly dateTimeOffset)
    {
        var ct = TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time");
        return new DateTimeOffset(dateTimeOffset, default, ct.GetUtcOffset(dateTimeOffset.ToDateTime(default)))
            .ToUnixTimeSeconds();
    }

    private static DateOnly ToDateOnly(this long dateTimeOffset)
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