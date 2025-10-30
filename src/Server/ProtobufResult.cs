// Copyright (C) 2025 Micah Makaiwi
// 
// This program is free software: you can redistribute it and/or modify it under the terms of the GNU Affero General
// Public License as published by the Free Software Foundation, version 3.
// 
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY

using Google.Protobuf;

using MMKiwi.CtaTracker.Model.Protobuf;

namespace MMKiwi.CtaTracker.Server;

public static class ProtobufResult
{
    public static ProtobufResult<TInner,TOuter> Create<TInner,TOuter>(TOuter message)
        where TOuter : IMessage, IResponse<TInner>
        where TInner : IMessage
        => ProtobufResult<TInner,TOuter>.Create(message);
}


public class ProtobufResult<TInner,TOuter> : IResult
    where TOuter : IMessage, IResponse<TInner>
    where TInner : IMessage
{
    private ProtobufResult(TOuter message)
    {
        Message = message;
    }

    public static ProtobufResult<TInner,TOuter> Create(TOuter message) => new(message);

    private TOuter Message { get; }

    public async Task ExecuteAsync(HttpContext httpContext)
    {
        IMessage response;
        if (Message.HasErrors)
        {
            httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            response = Message.Errors;
        }
        else
        {
            httpContext.Response.StatusCode = StatusCodes.Status200OK;
            response = Message.Response;
        }
        
        httpContext.Response.ContentType = "application/protobuf";
        var responseBytes = response.ToByteArray();
        httpContext.Response.ContentLength = responseBytes.Length;
        await httpContext.Response.BodyWriter.WriteAsync(responseBytes);
        await httpContext.Response.BodyWriter.CompleteAsync();
    }
}