# WebResultEndpoint Response Mapping

`WebResultEndpoint` transforms the result returned by its `HandleAsync` method —a `WebResult` object encapsulating business result— into appropriate HTTP status code and response format, providing consistent and type-safe API behavior. This process relies on `ExecuteAsync` method implementation of the returned `WebResult` object.

There are several ways to customize how responses are mapped:

- One way is to implement a custom class inheriting abstract `WebResult`, where you override `ExecuteAsync` method to provide your custom mapping logic. Then return an instance of the custom `WebResult` class from the endpoint handler method,

- Another way to customize response mapping is by overriding the ConvertResultToResponseAsync method. This method is invoked after HandleAsync to transform the business result into an HTTP response. By default, it just calls the ExecuteAsync method of the returned WebResult object, but you can override it to implement your own mapping logic.

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
