// Copyright (C) 2025 Micah Makaiwi
// 
// This program is free software: you can redistribute it and/or modify it under the terms of the GNU Affero General
// Public License as published by the Free Software Foundation, version 3.
// 
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY

using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

using NodaTime;
using NodaTime.Text;

namespace MMKiwi.CtaTracker.Model;

public partial class UnixToZonedDateTimeConverter : JsonConverter<ZonedDateTime>
{
    [field:MaybeNull]
    public static DateTimeZone CentralTime => field ??= DateTimeZoneProviders.Tzdb["America/Chicago"];
    
    public override ZonedDateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return ToZonedDateTime(reader.GetString());
    }

    public static ZonedDateTime ToZonedDateTime(ReadOnlySpan<char> time)
    {
        if (Numeric.IsMatch(time)) // Unix time
        {
            return Instant.FromUnixTimeMilliseconds(long.Parse(time)).InZone(CentralTime);
        }
        LocalDateTimePattern format = time.Length switch
        {
            8 => LocalDateTimePattern.Create("yyyyMMdd", CultureInfo.InvariantCulture),
            14 => LocalDateTimePattern.Create("yyyyMMdd HH:mm", CultureInfo.InvariantCulture),
            17 => LocalDateTimePattern.Create("yyyyMMdd HH:mm:ss", CultureInfo.InvariantCulture),
            _ => throw new FormatException($"{time} is an invalid date format")
        };

        return format.Parse(new string(time)).Value.InZoneStrictly(CentralTime);
    }

    // write is out of scope, but this could be implemented via writer.ToUnixTimeMilliseconds/WriteNullValue
    public override void Write(Utf8JsonWriter writer, ZonedDateTime value, JsonSerializerOptions options) =>
        writer.WriteNumberValue(value.ToInstant().ToUnixTimeSeconds());
    
    [GeneratedRegex("^\\d+$")] private static partial Regex Numeric { get; }
}