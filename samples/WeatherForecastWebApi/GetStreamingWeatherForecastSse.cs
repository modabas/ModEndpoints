using System.Net.ServerSentEvents;
using System.Runtime.CompilerServices;
using FluentValidation;
using ModEndpoints.Core;

namespace WeatherForecastWebApi;

internal class GetStreamingWeatherForecastSse
  : MinimalEndpoint<IResult>
{
  private static readonly string[] _summaries = ["Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"];

  protected override void Configure(
    EndpointConfigurationBuilder builder,
    ConfigurationContext<EndpointConfigurationParameters> configurationContext)
  {
    builder.MapGet("/streamingweatherforecastsse")
      .WithName("GetStreamingWeatherForecastSse")
      .WithTags("WeatherForecastWebApi")
      .Produces<SseItem<WeatherForecast>>(contentType: "text/event-stream");
  }

  protected override async Task<IResult> HandleAsync(CancellationToken ct)
  {
    await Task.CompletedTask;
    return Results.ServerSentEvents<WeatherForecast>(GetForecast(ct));

    async IAsyncEnumerable<WeatherForecast> GetForecast([EnumeratorCancellation] CancellationToken ct)
    {
      var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            _summaries[Random.Shared.Next(_summaries.Length)]
        ))
        .ToArray();

      foreach (var item in forecast)
      {
        yield return item;
        await Task.Delay(500, ct);
      }
    }
  }
}
