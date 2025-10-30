// Copyright (C) 2025 Micah Makaiwi
// 
// This program is free software: you can redistribute it and/or modify it under the terms of the GNU Affero General
// Public License as published by the Free Software Foundation, version 3.
// 
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY

using System.Text.Json.Serialization;

namespace MMKiwi.CtaTracker.Model.Alerts;

public record Alert(
    [property: JsonPropertyName("AlertId")] string AlertId,
    [property: JsonPropertyName("Headline")] string Headline,
    [property: JsonPropertyName("ShortDescription")] string ShortDescription,
    [property: JsonPropertyName("FullDescription")] FullDescription FullDescription,
    [property: JsonPropertyName("SeverityScore")] string SeverityScore,
    [property: JsonPropertyName("SeverityColor")] string SeverityColor,
    [property: JsonPropertyName("SeverityCSS")] string SeverityCSS,
    [property: JsonPropertyName("Impact")] string Impact,
    [property: JsonPropertyName("EventStart")] object EventStart,
    [property: JsonPropertyName("EventEnd")] DateTime? EventEnd,
    [property: JsonPropertyName("TBD")] string TBD,
    [property: JsonPropertyName("MajorAlert")] string MajorAlert,
    [property: JsonPropertyName("AlertURL")] AlertURL AlertURL,
    //[property: JsonPropertyName("ImpactedService")] ImpactedService ImpactedService,
    [property: JsonPropertyName("ttim")] string ttim,
    [property: JsonPropertyName("GUID")] string GUID
);