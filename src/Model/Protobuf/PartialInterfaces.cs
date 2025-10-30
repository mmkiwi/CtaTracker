// Copyright (C) 2025 Micah Makaiwi
// 
// This program is free software: you can redistribute it and/or modify it under the terms of the GNU Affero General
// Public License as published by the Free Software Foundation, version 3.
// 
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY

namespace MMKiwi.CtaTracker.Model.Protobuf;

public sealed partial class DetourResponse:IResponse
{
    public bool HasErrors => this.ResponseCase == ResponseOneofCase.Error;
    IEnumerable<string> IResponse.Errors => Error.Message;
}

public sealed partial class DirectionResponse:IResponse
{
    public bool HasErrors => this.ResponseCase == ResponseOneofCase.Error;
    IEnumerable<string> IResponse.Errors => Error.Message;
}

public sealed partial class LocaleResponse:IResponse
{
    public bool HasErrors => this.ResponseCase == ResponseOneofCase.Error;
    IEnumerable<string> IResponse.Errors => Error.Message;
}

public sealed partial class PatternResponse:IResponse
{
    public bool HasErrors => this.ResponseCase == ResponseOneofCase.Error;
    IEnumerable<string> IResponse.Errors => Error.Message;
}

public sealed partial class PredictionResponse:IResponse
{
    public bool HasErrors => this.ResponseCase == ResponseOneofCase.Error;
    IEnumerable<string> IResponse.Errors => Error.Message;
}

public sealed partial class RouteResponse:IResponse
{
    public bool HasErrors => this.ResponseCase == ResponseOneofCase.Error;
    IEnumerable<string> IResponse.Errors => Error.Message;
}
public sealed partial class StopResponse:IResponse
{
    public bool HasErrors => this.ResponseCase == ResponseOneofCase.Error;
    IEnumerable<string> IResponse.Errors => Error.Message;
}

public sealed partial class TimeResponse:IResponse
{
    public bool HasErrors => this.ResponseCase == ResponseOneofCase.Error;
    IEnumerable<string> IResponse.Errors => Error.Message;
}

public sealed partial class VehicleResponse:IResponse
{
    public bool HasErrors => this.ResponseCase == ResponseOneofCase.Error;
    IEnumerable<string> IResponse.Errors => Error.Message;
}

public interface IResponse
{
    bool HasErrors { get;  }
    IEnumerable<string> Errors { get; }
}