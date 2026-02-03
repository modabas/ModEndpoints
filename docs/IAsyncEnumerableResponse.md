# IAsyncEnumerable Response

## 1. MinimalEndpointWithStreamingResponse

`MinimalEndpointWithStreamingResponse` is a specialized base class designed to simplify the implementation of minimal APIs that return streaming responses in .NET. It provides a structured way to define endpoints that stream data asynchronously using `IAsyncEnumerable<T>`.

> **Note:** .Net 10 introduced `ServerSentEventsResult` IResult type to return IAsyncEnumerable responses which can be used by a `MinimalEndpoint`. See below for more information.

``` csharp
public record ListCustomersResponse(Guid Id, string FirstName, string? MiddleName, string LastName);

internal class ListCustomers(ServiceDbContext db)
  : MinimalEndpointWithStreamingResponse<ListCustomersResponse>
{
  protected override void Configure(
    EndpointConfigurationBuilder builder,
    EndpointConfigurationContext configurationContext)
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

### *For .Net 10 or Later*

By utilizing `ServerSentEventResult`, it's possible to return streaming responses with `MinimalEndpoint`.
```csharp
internal class GetStreamingWeatherForecastSse
  : MinimalEndpoint<IResult>
{
  private static readonly string[] _summaries = ["Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"];

  protected override void Configure(
    EndpointConfigurationBuilder builder,
    EndpointConfigurationContext configurationContext)
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
    EndpointConfigurationContext configurationContext)
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

## 2. WebResultEndpoint with IAsyncEnumerable Response

`WebResultEndpoint` can also return streaming responses using `IAsyncEnumerable<TValue>`.

> **Note:** .Net 10 introduced `ServerSentEventsResult` IResult type to return IAsyncEnumerable responses which can be used by a `WebResultEndpoint`. See below for more information.

``` csharp
internal class ListBooksWithStreamingResponse
  : WebResultEndpointWithEmptyRequest<IAsyncEnumerable<ListBooksResponseItem>>
{
  protected override void Configure(
    EndpointConfigurationBuilder builder,
    EndpointConfigurationContext configurationContext)
  {
    builder.MapGet("/listWithStreamingResponse/");
  }

  protected override async Task<WebResult<IAsyncEnumerable<ListBooksResponseItem>>> HandleAsync(
    CancellationToken ct)
  {
    return WebResults.WithStreamingResponse(GetBooks(ct));

    async IAsyncEnumerable<ListBooksResponseItem> GetBooks(
      [EnumeratorCancellation] CancellationToken ct)
    {
      List<ListBooksResponseItem> books =
        [
          new ListBooksResponseItem(Guid.NewGuid(), "Book 1", "Author 1", 19.99m),
          new ListBooksResponseItem(Guid.NewGuid(), "Book 2", "Author 2", 29.99m)
        ];
      foreach (var book in books)
      {
        yield return book;
        await Task.Delay(1000, ct);
      }
    }
  }
}
```

### *For .Net 10 or Later*

By utilizing `ServerSentEventResult`, it's possible to return streaming responses with `WebResultEndpoint`. In this case streaming responses are wrapped in `SseItem<TValue>` to provide additional metadata for each streamed item.

``` csharp
internal class ListBooksSse
  : WebResultEndpointWithEmptyRequest<IAsyncEnumerable<SseItem<ListBooksResponseItem>>>
{
  protected override void Configure(
    EndpointConfigurationBuilder builder,
    EndpointConfigurationContext configurationContext)
  {
    builder.MapGet("/listSse/")
      .Produces<SseItem<ListBooksResponseItem>>(contentType: "text/event-stream");
  }

  protected override async Task<WebResult<IAsyncEnumerable<SseItem<ListBooksResponseItem>>>> HandleAsync(
    CancellationToken ct)
  {
    return WebResults.ServerSentEvents(GetBooks(ct));

    async IAsyncEnumerable<ListBooksResponseItem> GetBooks(
      [EnumeratorCancellation] CancellationToken ct)
    {
      List<ListBooksResponseItem> books =
        [
          new ListBooksResponseItem(Guid.NewGuid(), "Book 1", "Author 1", 19.99m),
          new ListBooksResponseItem(Guid.NewGuid(), "Book 2", "Author 2", 29.99m)
        ];
      foreach (var book in books)
      {
        yield return book;
        await Task.Delay(1000, ct);
      }
    }
  }
}
```

## 3. ServiceEndpointWithStreamingResponse

Similarly, `ServiceEndpointWithStreamingResponse` endpoint type can be used to implement service endpoints that return streaming responses.
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
      yield return new StreamingResponseItem<ListStoresResponse>(store, "store");
      await Task.Delay(1000, ct);
    }
  }
}

public record ListStoresRequest() : IServiceRequestWithStreamingResponse<ListStoresResponse>;
public record ListStoresResponse(Guid Id, string Name);

```