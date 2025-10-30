// Copyright (C) 2025 Micah Makaiwi
// 
// This program is free software: you can redistribute it and/or modify it under the terms of the GNU Affero General
// Public License as published by the Free Software Foundation, version 3.
// 
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY

using System.Buffers;
using System.Diagnostics.CodeAnalysis;

namespace MMKiwi.CtaTracker.Server;

public class BadAccept : IResult
{
    public Task ExecuteAsync(HttpContext httpContext)
    {
        return Results
            .BadRequest("ERROR 400\nPlease provide a valid content type (application/json or application/protobuf)")
            .ExecuteAsync(httpContext);
    }

    [field: MaybeNull] public static BadAccept Instance => field ??= new BadAccept();
}