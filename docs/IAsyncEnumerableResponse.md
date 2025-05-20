# IAsyncEnumerable Response

`MinimalEndpointWithStreamingResponse` is a specialized base class designed to simplify the implementation of minimal APIs that return streaming responses in .NET. It provides a structured way to define endpoints that stream data asynchronously using `IAsyncEnumerable<T>`.

``` csharp
public record ListCustomersResponse(
  Guid Id,
  string FirstName,
  string? MiddleName,
  string LastName);

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