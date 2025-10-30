// Copyright (C) 2025 Micah Makaiwi
// 
// This program is free software: you can redistribute it and/or modify it under the terms of the GNU Affero General
// Public License as published by the Free Software Foundation, version 3.
// 
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY

using Google.Protobuf;

namespace MMKiwi.CtaTracker.Model.Protobuf;

public partial class PredictionResponse : IResponse<PredictionResponse.Types.RouteList>
{
    public bool HasErrors => this.ResponseCase == ResponseOneofCase.Error;
    ErrorList IResponse<Types.RouteList>.Errors => Error;
    Types.RouteList IResponse<Types.RouteList>.Response => this.Result;
    
}

public sealed partial class TimeResponse : IResponse<TimeResponse.Types.Time>
{
    public bool HasErrors => this.ResponseCase == ResponseOneofCase.Error;
    Types.Time IResponse<Types.Time>.Response => this.Result;
    ErrorList IResponse<Types.Time>.Errors => Error;
}

public interface IResponse<out T>
    where T:IMessage
{
    bool HasErrors { get; }
    ErrorList Errors { get; }

    T Response { get; }
}