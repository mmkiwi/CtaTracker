using System.Net.Security;
using System.Text.Json.Serialization;

using Google.Protobuf;

using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;

using MMKiwi.CtaTracker.Client;
using MMKiwi.CtaTracker.Model.Protobuf;

namespace MMKiwi.CtaTracker.Server;

public partial class Program
{
    public static void Main(string[] args)
    {
        
        var builder = WebApplication.CreateSlimBuilder(args);

        builder.Services.ConfigureHttpJsonOptions(void (options) =>
        {
            options.SerializerOptions.TypeInfoResolverChain.Insert(0, BusSerializer.Default);
            options.SerializerOptions.TypeInfoResolverChain.Insert(0, Serializer.Default);
        });

        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApi();
        builder.Services.AddSingleton(_ => new ConfigurationBuilder()
            .AddUserSecrets<Program>()
            .Build());
        builder.Services.AddMemoryCache();
        builder.Services.AddSingleton<BusTrackerClient>(c =>
        {
            var config = c.GetRequiredService<IConfigurationRoot>();
            var key = config["BusTrackerKey"] ??
                      throw new InvalidOperationException(
                          "You must provide a valid secret key BusTrackerKey in usersecrets");
            return new BusTrackerClient(key);
        });
        builder.Services.AddSingleton<Controller>();
        var app = builder.Build();
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }
        
        app.UseForwardedHeaders(new ForwardedHeadersOptions
        {
            ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
        });

        var controller = app.Services.GetRequiredService<Controller>();


        try
        {
            var bus = app.MapGroup("/tracker");
            bus.MapGet("/serverTime", controller.ServerTime)
                .WithName("Get Server Time");
            bus.MapGet("/prediction", controller.GetPredictions).WithName("Get Routes");


            app.Run();
        }
        finally
        {
            controller.Dispose();
        }
    }

    [JsonSerializable(typeof(IReadOnlyList<string>))]
    [JsonSerializable(typeof(string[]))]
    [JsonSerializable(typeof(PredictionResponse))]
    [JsonSerializable(typeof(TimeResponse))]
    [JsonSerializable(typeof(TimeResponse.ResponseOneofCase), TypeInfoPropertyName = "TimeResponseCase")]
    [JsonSerializable(typeof(PredictionResponse.ResponseOneofCase), TypeInfoPropertyName = "PredictionResponseCase")]
    private partial class Serializer : JsonSerializerContext;
}