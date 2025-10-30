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

using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace MMKiwi.CtaTracker.Model;

[Obsolete]
public partial class UnixToDateTimeConverter : JsonConverter<DateTimeOffset>
{
    public override DateTimeOffset Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return ToDateTime(reader.GetString());
    }
    
    public static DateTimeOffset ToDateTime(ReadOnlySpan<char> time)
    {
        var ct = TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time");
        
        if (Numeric.IsMatch(time)) // Unix time
        {
            var dt = DateTimeOffset.FromUnixTimeMilliseconds(long.Parse(time));
            return dt.ToOffset(ct.GetUtcOffset(dt));
        }
        string format = time.Length switch
        {
            8 => "yyyyMMdd",
            14 => "yyyyMMdd HH:mm",
            17 => "yyyyMMdd HH:mm:ss",
            _ => throw new FormatException($"{time} is an invalid date format")
        };

        var pattern = DateTime.ParseExact(time, format, CultureInfo.InvariantCulture);
        
        return new DateTimeOffset(pattern, ct.GetUtcOffset(pattern));
    }

    // write is out of scope, but this could be implemented via writer.ToUnixTimeMilliseconds/WriteNullValue
    public override void Write(Utf8JsonWriter writer, DateTimeOffset value, JsonSerializerOptions options) =>
        writer.WriteNumberValue(value.ToUnixTimeSeconds());
    
    [GeneratedRegex("^\\d+$")] private static partial Regex Numeric { get; }
}