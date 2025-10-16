using ModEndpoints.Core;

namespace WeatherForecastWebApi;

internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
  public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}

internal class GetWeatherForecast : MinimalEndpoint<WeatherForecast[]>
{
  private static readonly string[] _summaries = ["Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"];

  protected override void Configure(
    EndpointConfigurationBuilder builder,
    ConfigurationContext<EndpointConfigurationParameters> configurationContext)
  {
    builder.MapGet("/weatherforecast")
      .WithName("GetWeatherForecast")
      .WithTags("WeatherForecastWebApi");
  }

  protected override Task<WeatherForecast[]> HandleAsync(CancellationToken ct)
  {
    var forecast = Enumerable.Range(1, 5).Select(index =>
      new WeatherForecast
      (
          DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
          Random.Shared.Next(-20, 55),
          _summaries[Random.Shared.Next(_summaries.Length)]
      ))
      .ToArray();

    return Task.FromResult(forecast);
  }
}
