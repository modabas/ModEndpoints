# Route Groups

By default, all endpoints are mapped under root route group. It is possible to define route groups similar to using 'MapGroup' extension method used to map Minimal APIs under a group. Since endpoints are configured by endpoint basis in the 'Configure' method of each endpoint, the approach is a little different than configuring a Minimal API. But route group configuration still utilize Minimal API route groups and can be decorated by any extension method of RouteGroupBuilder. Route groups are also subject to auto discovery and registration, similar to endpoints.

- [Create a route group implementation](../samples/ShowcaseWebApi/Features/FeaturesRouteGroup.cs) by inheriting RouteGroupConfigurator and implementing 'Configure' method,
- Configuration of each route group implementation starts with calling input parameter `builder`'s MapGroup method with a route pattern prefix. The return of 'MapGroup' method, a RouteGroupBuilder instance, can be used to further customize the route group like any Minimal API route group.
- Apply MapToGroup attribute to either other [route group](../samples/ShowcaseWebApi/Features/Books/Configuration/BooksV1RouteGroup.cs) or [endpoint](../samples/ShowcaseWebApi/Features/Books/CreateBook.cs) classes that will be mapped under created route group. Use type of the new route group implementation as GroupType parameter to the attribute.

Following sample creates a parent route group (FeaturesRouteGroup), a child route group under it (BooksV1RouteGroup) and maps an endpoint (CreateBook) to child route group. Group configuration methods used for this particular sample are all part of Minimal APIs ecosystem and are under [Asp.Versioning](https://github.com/dotnet/aspnet-api-versioning).

> **Note**: Dependencies resolved from constructor are not available during configuration. To access a service from dependency injection in the configuration phase, use the `ServiceProvider` property of the configuration context parameter provided to the `Configure` method.

```csharp
internal class FeaturesRouteGroup : RouteGroupConfigurator
{
  protected override void Configure(
    RouteGroupConfigurationBuilder builder,
    RouteGroupConfigurationContext configurationContext)
  {
    var groupBuilder = builder.MapGroup("/api/v{version:apiVersion}");
    var apiVersionSet = groupBuilder.NewApiVersionSet()
      .HasApiVersion(new ApiVersion(1))
      .HasApiVersion(new ApiVersion(2))
      .ReportApiVersions()
      .Build();
    groupBuilder.WithApiVersionSet(apiVersionSet);
  }
}

[MapToGroup<FeaturesRouteGroup>()]
internal class BooksV1RouteGroup : RouteGroupConfigurator
{
  protected override void Configure(
    RouteGroupConfigurationBuilder builder,
    RouteGroupConfigurationContext configurationContext)
  {
    builder.MapGroup("/books")
      .MapToApiVersion(1)
      .WithTags("/BooksV1");
  }
}

[MapToGroup<BooksV1RouteGroup>()]
internal class CreateBook(ServiceDbContext db, ILocationStore location)
  : WebResultEndpoint<CreateBookRequest, CreateBookResponse>
{
  protected override void Configure(
    EndpointConfigurationBuilder builder,
    EndpointConfigurationContext configurationContext)
  {
    builder.MapPost("/")
      .Produces<CreateBookResponse>(StatusCodes.Status201Created);
  }
  protected override async Task<WebResult<CreateBookResponse>> HandleAsync(
    CreateBookRequest req,
    CancellationToken ct)
  {
    //Handle...
  }
}
```

