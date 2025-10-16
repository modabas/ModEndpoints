using System.Runtime.CompilerServices;
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using ModEndpoints.Core;

namespace WeatherForecastWebApi;

internal class GetStreamingWeatherForecastTypedSse
  : MinimalEndpoint<Results<ServerSentEventsResult<WeatherForecast>, ProblemHttpResult>>
{
  private static readonly string[] _summaries = ["Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"];

  protected override void Configure(
    EndpointConfigurationBuilder builder,
    ConfigurationContext<EndpointConfigurationParameters> configurationContext)
  {
    builder.MapGet("/streamingweatherforecasttypedsse")
      .WithName("GetStreamingWeatherForecastTypedSse")
      .WithTags("WeatherForecastWebApi");
  }

  protected override async Task<Results<ServerSentEventsResult<WeatherForecast>, ProblemHttpResult>> HandleAsync(CancellationToken ct)
  {
    await Task.CompletedTask;
    return TypedResults.ServerSentEvents<WeatherForecast>(GetForecast(ct));

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
