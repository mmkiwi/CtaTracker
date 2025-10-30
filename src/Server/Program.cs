using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http.Headers;

using Google.Protobuf;

using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;

using MMKiwi.CtaTracker.Client;
using MMKiwi.CtaTracker.Model;
using MMKiwi.CtaTracker.Model.BusTracker;
using MMKiwi.CtaTracker.Model.Protobuf;

using Route = MMKiwi.CtaTracker.Model.BusTracker.Route;

namespace MMKiwi.CtaTracker.Server;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateSlimBuilder(args);

        builder.Services.ConfigureHttpJsonOptions(void (options) =>
        {
            options.SerializerOptions.TypeInfoResolverChain.Insert(0, BusSerializer.Default);
        });

        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApi();
        var app = builder.Build();
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        var config = new ConfigurationBuilder()
            .AddUserSecrets<Program>()
            .Build();
        var key = config["BusTrackerKey"] ??
                  throw new InvalidOperationException(
                      "You must provide a valid secret key BusTrackerKey in usersecrets");
        HttpClient client = new();
        BusTrackerClient busClient = new(key, client, false);

        try
        {
            var bus = app.MapGroup("/bus");
            bus.MapGet("/serverTime", async Task<IResult> (HttpRequest request) =>
                {
                    Response<ServerTime> ctaResponse = await busClient.GetTime();
                    return request.PerferedOutputFormat switch
                    {
                        PerferedOutputFormat.Json => JsonResult.Create(ctaResponse),
                        PerferedOutputFormat.Protobuf => ProtobufResult.Create(ctaResponse.ToProtobuf()),
                        _ => BadAccept.Instance
                    };
                })
                .WithName("Get Server Time");
            bus.MapGet("/routes", async Task<IResult> (HttpRequest request) =>
            {
                Response<IReadOnlyList<Route>> ctaResponse = await busClient.GetRoutes();
                return request.PerferedOutputFormat switch
                {
                    PerferedOutputFormat.Json => JsonResult.Create(ctaResponse),
                    PerferedOutputFormat.Protobuf => ProtobufResult.Create(ctaResponse.ToProtobuf()),
                    _ => BadAccept.Instance
                };
            }).WithName("Get Routes");


            app.Run();
        }
        finally
        {
            client.Dispose();
        }
    }
}

public static class ProtobufResult
{
    public static ProtobufResult<T> Create<T>(T message)
        where T : IMessage, IResponse
        => ProtobufResult<T>.Create(message);
}

public class ProtobufResult<T> : IResult
    where T : IMessage, IResponse
{
    private ProtobufResult(T message)
    {
        Message = message;
    }

    public static ProtobufResult<T> Create(T message) => new(message);

    private T Message { get; }

    public async Task ExecuteAsync(HttpContext httpContext)
    {
        httpContext.Response.StatusCode = Message.HasErrors ? StatusCodes.Status400BadRequest : StatusCodes.Status200OK;
        httpContext.Response.ContentType = "application/protobuf";
        var response = Message.ToByteArray();
        httpContext.Response.ContentLength = response.Length;
        await httpContext.Response.BodyWriter.WriteAsync(response);
        await httpContext.Response.BodyWriter.CompleteAsync();
    }
}

public static class JsonResult
{
    public static JsonResult<TInner> Create<TInner>(Response<TInner> message)
        where TInner : class
        => JsonResult<TInner>.Create(message);
}

public class JsonResult<T> : IResult
    where T : class
{
    private JsonResult(Response<T> message)
    {
        Message = message;
    }

    public static JsonResult<T> Create(Response<T> message) => new(message);

    private Response<T> Message { get; }

    public async Task ExecuteAsync(HttpContext httpContext)
    {
        if (Message.Errors is {Count: > 0})
        {
            await Results.BadRequest(Message.Errors).ExecuteAsync(httpContext);
        }
        else
        {
            await Results.Ok(Message.Result).ExecuteAsync(httpContext);
        }
    }
}

public class BadAccept : IResult
{
    public Task ExecuteAsync(HttpContext httpContext)
    {
        httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
        httpContext.Response.ContentType = "text/plain";
        httpContext.Response.BodyWriter.Write(
            "ERROR 400\nPlease provide a valid content type (application/json or application/protobuf)"u8);
        return Task.CompletedTask;
    }

    [field: MaybeNull] public static BadAccept Instance => field ??= new BadAccept();
}

public enum PerferedOutputFormat
{
    Unknown,
    Json,
    Protobuf
}

public static class Extensions
{
    extension(HttpRequest request)
    {
        public PerferedOutputFormat PerferedOutputFormat
        {
            get
            {
                if (request.Query.TryGetValue("format", out StringValues format))
                {
                    if (format == "json")
                        return PerferedOutputFormat.Json;
                    if (format == "protobuf")
                        return PerferedOutputFormat.Protobuf;
                }
                else if (request.Headers.TryGetValue(HeaderNames.Accept, out StringValues accept))
                {
                    IEnumerable<MediaTypeWithQualityHeaderValue> accepted = accept.First()?.Split(',')
                                                                                .Select(MediaTypeWithQualityHeaderValue
                                                                                    .Parse)
                                                                                .OrderByDescending(mt =>
                                                                                    mt.Quality.GetValueOrDefault(1)) ??
                                                                            Enumerable
                                                                                .Empty<
                                                                                    MediaTypeWithQualityHeaderValue>();
                    foreach (var acceptedFormat in accepted)
                    {
                        if (acceptedFormat.MediaType?.EndsWith("protobuf") ?? false)
                            return PerferedOutputFormat.Protobuf;
                        else if (acceptedFormat.MediaType?.EndsWith("json") ?? false)
                            return PerferedOutputFormat.Json;
                    }
                }

                return PerferedOutputFormat.Unknown;
            }
        }
    }
}