// Copyright (C) 2025 Micah Makaiwi
// 
// This program is free software: you can redistribute it and/or modify it under the terms of the GNU Affero General
// Public License as published by the Free Software Foundation, version 3.
// 
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY

using System.Text.Json.Serialization;

namespace MMKiwi.CtaTracker.Model.Alerts;

public record CTAAlerts(
    [property: JsonPropertyName("TimeStamp")] DateTime TimeStamp,
    [property: JsonPropertyName("ErrorCode")] string ErrorCode,
    [property: JsonPropertyName("ErrorMessage")] object ErrorMessage,
    [property: JsonPropertyName("Alert")] IReadOnlyList<Alert> Alert
);