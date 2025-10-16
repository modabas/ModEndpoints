# IAsyncEnumerable Response

`MinimalEndpointWithStreamingResponse` is a specialized base class designed to simplify the implementation of minimal APIs that return streaming responses in .NET. It provides a structured way to define endpoints that stream data asynchronously using `IAsyncEnumerable<T>`.

> **Note:** .Net 10 introduced `ServerSentEventsResult` IResult type to return IAsyncEnumerable responses which can be used by a `MinimalEndpoint`. See below for more information.

``` csharp
public record ListCustomersResponse(Guid Id, string FirstName, string? MiddleName, string LastName);

internal class ListCustomers(ServiceDbContext db)
  : MinimalEndpointWithStreamingResponse<ListCustomersResponse>
{
  protected override void Configure(
    EndpointConfigurationBuilder builder,
    ConfigurationContext<EndpointConfigurationParameters> configurationContext)
  {
    builder.MapGet("/");
  }

  protected override IAsyncEnumerable<ListCustomersResponse> HandleAsync(CancellationToken ct)
  {
    var customers = db.Customers
      .Select(c => new ListCustomersResponse(
        c.Id,
        c.FirstName,
        c.MiddleName,
        c.LastName))
      .AsAsyncEnumerable();

    return customers;
  }
}
```
>**Note**: The CancellationToken passed to the `HandleAsync` method, is also used to cancel enumeration of the returned `IAsyncEnumerable<T>` by the internals of base endpoint.

Similarly, `ServiceEndpointWithStreamingResponse` can be used to implement service endpoints that return streaming responses.
``` csharp
internal class ListStores(ServiceDbContext db)
  : ServiceEndpointWithStreamingResponse<ListStoresRequest, ListStoresResponse>
{
  protected override async IAsyncEnumerable<StreamingResponseItem<ListStoresResponse>> HandleAsync(
    ListStoresRequest req,
    [EnumeratorCancellation] CancellationToken ct)
  {
    var stores = db.Stores
      .Select(b => new ListStoresResponse(
        b.Id,
        b.Name))
      .AsAsyncEnumerable();

    await foreach (var store in stores.WithCancellation(ct))
    {
      ct.ThrowIfCancellationRequested();
      yield return new StreamingResponseItem<ListStoresResponse>(store, "store");
      await Task.Delay(1000, ct);
    }
  }
}
```

## For .Net 10 or Later

By utilizing `ServerSentEventResult`, it's possible to return streaming responses with `MinimalEndpoint`.
```csharp
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
```

Instead of using `IResult` response type, it's also possible to use TypedResults for a more type safe approach.
```csharp
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
```