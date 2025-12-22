# WebResultEndpoint Response Mapping

`WebResultEndpoint` transforms the result returned by its `HandleAsync` method —a `WebResult` object encapsulating business result— into appropriate HTTP status code and response format, providing consistent and type-safe API behavior. This process relies on `ExecuteAsync` method implementation of the returned `WebResult` object. Since encapsulated business result may be in Ok or Failed state with any failure type, `ExecuteAsync` method is responsible for mapping different business result states to corresponding HTTP responses.

`WebResults` static class provides factory methods to create various `WebResult` implementations for common scenarios:

- `FromResult` method: This creates a default implementation of `WebResult`.
- `WithLocationUriOnSuccess` method: This creates a `WebResult` that returns a `Location` header set to provided Uri when mapped HTTP response is either `201 Created` or `202 Accepted`. Otherwise, it behaves like a default `WebResult`.
- `WithLocationRouteOnSuccess` method: This creates a `WebResult` that returns a `Location` header based on a named route when mapped HTTP response is either `201 Created` or `202 Accepted`. Otherwise, it behaves like a default `WebResult`.

## Customizing Response Mapping

There are several ways to customize how responses are mapped:

- One way is to implement a custom class inheriting abstract `WebResult`, where you override `ExecuteAsync` method to provide your custom mapping logic. Then return an instance of the custom `WebResult` class from the endpoint handler method,

- Another way to customize response mapping is by overriding the `ConvertResultToResponseAsync` method. This method is invoked 
  1. after `HandleAsync` to transform the business result into an HTTP response. 
  2. or after `HandleValidationFailureAsync` if request validation fails.

By default, `ConvertResultToResponseAsync` method just calls the ExecuteAsync method of the input parameter `WebResult` object, but you can override it to implement your own mapping logic.
 
``` csharp
internal class GetBookById(ServiceDbContext db)
  : WebResultEndpoint<GetBookByIdRequest, GetBookByIdResponse>
{
  protected override void Configure(
    EndpointConfigurationBuilder builder,
    EndpointConfigurationContext configurationContext)
  {
    builder.MapGet("/{Id}")
      .Produces<GetBookByIdResponse>();
  }

  protected override ValueTask<IResult> ConvertResultToResponseAsync(
    WebResult<GetBookByIdResponse> result,
    HttpContext context,
    CancellationToken ct)
  {
    // Custom mapping logic here

  }

  protected override async Task<WebResult<GetBookByIdResponse>> HandleAsync(
    GetBookByIdRequest req,
    CancellationToken ct)
  {
    // implementation

  }
}
```
