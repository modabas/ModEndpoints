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
    IServiceProvider serviceProvider,
    IRouteGroupConfigurator? parentRouteGroup)
  {
    MapGet("/");
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
>**Note**: The Cancellation Token is passed to the `HandleAsync` method, is also used to cancel enumeration of the returned `IAsyncEnumerable<T>` by the internals of base endpoint.