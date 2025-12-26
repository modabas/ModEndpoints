using System.Runtime.CompilerServices;
using FluentValidation;
using ModEndpoints.Core;

namespace WeatherForecastWebApi;

internal class GetStreamingWeatherForecast
  : MinimalEndpointWithStreamingResponse<WeatherForecast>
{
  private static readonly string[] _summaries = ["Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"];

  protected override void Configure(
    EndpointConfigurationBuilder builder,
    EndpointConfigurationContext configurationContext)
  {
    builder.MapGet("/streamingweatherforecast")
      .WithName("GetStreamingWeatherForecast")
      .WithTags("WeatherForecastWebApi");
  }

  protected override async IAsyncEnumerable<WeatherForecast> HandleAsync(
    [EnumeratorCancellation] CancellationToken ct)
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
