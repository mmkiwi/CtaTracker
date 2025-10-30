// Copyright (C) 2025 Micah Makaiwi
// 
// This program is free software: you can redistribute it and/or modify it under the terms of the GNU Affero General
// Public License as published by the Free Software Foundation, version 3.
// 
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY

using Google.Protobuf;

using MMKiwi.CtaTracker.Model.BusTracker;
using MMKiwi.CtaTracker.Model.Protobuf;

namespace MMKiwi.CtaTracker.Server;

public static class JsonResult
{
    public static JsonResult<TInner, TOuter> Create<TInner, TOuter>(TOuter message)
        where TOuter : IResponse<TInner>
        where TInner : IMessage
        => JsonResult<TInner, TOuter>.Create(message);
}

public class JsonResult<TInner, TOuter> : IResult
    where TOuter : IResponse<TInner>
    where TInner : IMessage
{
    private JsonResult(TOuter message)
    {
        Message = message;
    }

    public static JsonResult<TInner, TOuter> Create(TOuter message) => new(message);
    private TOuter Message { get; }

    public async Task ExecuteAsync(HttpContext httpContext)
    {
        if (Message.HasErrors)
        {
            await Results.BadRequest(Message.Errors).ExecuteAsync(httpContext);
        }
        else
        {
            await Results.Ok(Message.Response).ExecuteAsync(httpContext);
        }
    }
}