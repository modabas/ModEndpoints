# WebResultEndpoint Response Mapping

`WebResultEndpoint` transforms the result returned by its `HandleAsync` method —a business result— into appropriate HTTP status code and response format, providing consistent and type-safe API behavior. This process relies on a default result-to-response mapper.

There are several ways to customize how responses are mapped:

One way to customize response mapping is by overriding the ConvertResultToResponseAsync method. This method is invoked after HandleAsync to transform the business result into an HTTP response. By default, it uses the IResultToResponseMapper service for this conversion, but you can override it to implement your own mapping logic.

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
    Result<GetBookByIdResponse> result,
    HttpContext context,
    CancellationToken ct)
  {
    // Custom mapping logic here

  }

  protected override async Task<Result<GetBookByIdResponse>> HandleAsync(
    GetBookByIdRequest req,
    CancellationToken ct)
  {
    // implementation

  }
}
```

Another option is to implement your own `IResultToResponseMapper` and register it in the DI container as a keyed service using a string key.
You can then apply the `[ResultToResponseMapper("MyMapperKey")]` attribute to any `WebResultEndpoint`-based endpoint to have it use your custom mapper instead of the default one.

``` csharp
// Lifetime of the mapper is defined as singleton only for example purposes, it can be any lifetime
services.TryAddKeyedSingleton<IResultToResponseMapper, MyResultToResponseMapper>("MyMapper");
```

``` csharp
[ResultToResponseMapper("MyMapper")]
internal class GetBookById
  : WebResultEndpoint<GetBookByIdRequest, GetBookByIdResponse>
{
  protected override void Configure(
    EndpointConfigurationBuilder builder,
    EndpointConfigurationContext configurationContext)
  {
    builder.MapGet("/{Id}")
      .Produces<GetBookByIdResponse>();
  }

  protected override async Task<Result<GetBookByIdResponse>> HandleAsync(
    GetBookByIdRequest req,
    CancellationToken ct)
  {
    // implementation

  }
}
```

It is also possible to replace default mapper for all endpoints by registering your mapper as the default mapper before invoking any variant of `AddModEndpoints` method.

``` csharp
var builder = WebApplication.CreateBuilder(args);


// Register your custom mapper as the default mapper
// Lifetime of the mapper is defined as singleton only for example purposes, it can be any lifetime
services.TryAddKeyedSingleton<IResultToResponseMapper, MyResultToResponseMapper>(
  WebResultEndpointDefinitions.DefaultResultToResponseMapperName);

builder.Services.AddModEndpointsFromAssemblyContaining<MyEndpoint>();

//Register validators (from FluentValidation.DependencyInjectionExtensions nuget package, not included)
builder.Services.AddValidatorsFromAssemblyContaining<MyValidator>(includeInternalTypes: true);

var app = builder.Build();

app.MapModEndpoints();

app.Run();
```
