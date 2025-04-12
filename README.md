# ModEndpoints

[![Nuget downloads](https://img.shields.io/nuget/v/ModEndpoints.svg)](https://www.nuget.org/packages/ModEndpoints/)
[![Nuget](https://img.shields.io/nuget/dt/ModEndpoints)](https://www.nuget.org/packages/ModEndpoints/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://github.com/modabas/ModEndpoints/blob/main/LICENSE.txt)

[MinimalEndpoints](#minimalendpoint) are the barebone implementation for organizing ASP.NET Core Minimal Apis in REPR format endpoints. Their handler methods may return Minimal Api IResult based, string or T (any other type) response. MinimalEnpoints are implemented in ModEndpoints.Core package.

[WebResultEndpoints](#webresultendpoint), [BusinessResultEndpoints](#businessresultendpoint) and [ServiceEndpoints](#serviceendpoint) organize ASP.NET Core Minimal Apis in REPR format endpoints and are integrated with [result](https://github.com/modabas/ModResults) pattern out of box. They are implemented in ModEndpoints package.

To make consuming a [ServiceEndpoint](#serviceendpoint) easier, which is a very specialized endpoint more suitable for internal services, a specific [client implementation](#serviceendpoint-clients) along with extensions required for client registration is implemented in ModEndpoints.RemoteServices package, and interfaces required for ServiceEndpoint request models are in ModEndpoints.RemoteServices.Core package.

Each of them are demonstrated in [sample projects](#samples).

All endpoint abstractions are a structured approach to defining endpoints in ASP.NET Core applications. They extend the Minimal Api pattern with reusable, testable, and consistent components for request handling, validation, and response mapping.

## Key Features

 - Organizes ASP.NET Core Minimal Apis in REPR pattern endpoints
 - Encapsulates endpoint behaviors like request validation, request handling, and response mapping*.
 - Supports anything that Minimal Apis does. Configuration, parameter binding, authentication, Open Api tooling, filters, etc. are all Minimal Apis under the hood.
 - Supports auto discovery and registration.
 - Has built-in validation support with [FluentValidation](https://github.com/FluentValidation/FluentValidation). If a validator is registered for request model, request is automatically validated before being handled.
 - Supports constructor dependency injection in endpoint implementations.
 - Enforces response model type safety in request handlers.

 *WebResultEndpoint abstracts the logic for converting business results into HTTP responses.
 
## Workflow

An endpoint must implement two virtual methods: Configure and HandleAsync. A ServiceEndpoint has a default implementation for Configure method, which can be overridden, so only has to implement HandleAsync.

### Configuration:

The 'Configure' method is called at application startup to define routes and associate them with handler methods (MapGet, MapPost, etc.). Minimal Api RouteHandlerBuilders returned from these methods can be used to further customize endpoints. 

ServiceEndpoints are always mapped as Post methods under a pattern determined by resolved services, but have a GetRouteHandlerBuilder method to be used in Configure override to further configure them.

### Request Handling:

The request is processed in 'HandleAsync' method which returns a strongly typed [business result](https://github.com/modabas/ModResults) or in case of MinimalEndpoints, return a Minimal Apis IResult. This business result is handled differently for each endpoint type before being sent to client.

## Quickstart

### Service Registration

Use AddModEndpointsFromAssembly or AddModEndpointsFromAssemblyContaining<T> extension method to register all endpoints defined in an assembly.

Optional: Use FluentValidation.DependencyInjectionExtensions package to add FluentValidation validators to dependency injection for request validation.

Use MapModEndpoint extension method to map registered endpoints.

These methods register and map services required for all endpoint types.

 ``` csharp
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddModEndpointsFromAssemblyContaining<MyEndpoint>();
//Validation
builder.Services.AddValidatorsFromAssemblyContaining<MyValidator>(includeInternalTypes: true);

var app = builder.Build();

app.MapModEndpoints();

app.Run();
```

### Write a Minimal Api in REPR format

A [MinimalEndpoint](#minimalendpoint) is the most straighforward way to define a Minimal Api in REPR format.

Configuration of each endpoint implementation starts with calling one of the MapGet, MapPost, MapPut, MapDelete and MapPatch methods with a route pattern string. The return from any of these methods, a RouteHandlerBuilder instance, can be used to further customize the endpoint like a regular Minimal Api.

The request is processed in 'HandleAsync' method. Request is passed to handler method as parameter after validation (if a validator is registered for request model). Handler method returns a response model or a string or a Minimal Api IResult based response.

``` csharp
public record HelloWorldRequest(string Name);

internal class HelloWorldRequestValidator : AbstractValidator<HelloWorldRequest>
{
  public HelloWorldRequestValidator()
  {
    RuleFor(x => x.Name)
      .NotEmpty()
      .MinimumLength(3)
      .MaximumLength(50);
  }
}

internal class HelloWorld
  : MinimalEndpoint<HelloWorldRequest, IResult>
{
  protected override void Configure(
    IServiceProvider serviceProvider,
    IRouteGroupConfigurator? parentRouteGroup)
  {
    MapGet("MinimalEndpoints/HelloWorld/{Name}")
      .Produces<string>();
  }

  protected override Task<IResult> HandleAsync(HelloWorldRequest req, CancellationToken ct)
  {
    return Task.FromResult(Results.Ok($"Hello, {req.Name}."));
  }
}
```

### Integration with result pattern: A GET WebResultEndpoint with empty request

A [WebResultEndpoint](#webresultendpoint) can be utilized to abstract the logic for converting business results into HTTP responses of endpoints. Configuration and request handling is similar to MinimalEndpoint, but a WebResultEndpoint handler method also has the benefit of having a strongly typed return while having potential to return different HTTP response codes according to business result state.

This sample demonstrates a GET endpoint with basic configuration and without any request model binding. Business result instance returned from handler method is converted to a Minimal Api IResult based response by WebResultEndpoint before being sent to client.

Have a look at [ShowcaseWebApi](https://github.com/modabas/ModEndpoints/tree/main/samples/ShowcaseWebApi) project for various kinds of endpoint implementations and configurations.

``` csharp
public record ListBooksResponse(List<ListBooksResponseItem> Books);

public record ListBooksResponseItem(Guid Id, string Title, string Author, decimal Price);

internal class ListBooks(ServiceDbContext db)
  : WebResultEndpointWithEmptyRequest<ListBooksResponse>
{
  protected override void Configure(
    IServiceProvider serviceProvider,
    IRouteGroupConfigurator? parentRouteGroup)
  {
    MapGet("/books")
      .Produces<ListBooksResponse>();
  }

  protected override async Task<Result<ListBooksResponse>> HandleAsync(
    CancellationToken ct)
  {
    var books = await db.Books
      .Select(b => new ListBooksResponseItem(
        b.Id,
        b.Title,
        b.Author,
        b.Price))
      .ToListAsync(ct);

    return new ListBooksResponse(Books: books);
  }
}
```

### Parameter binding

Request model defined for an endpoint is bound with [AsParameters] attribute (except for ServiceEndpoints). Any field under request model can be bound from route, query, body, form, etc. with corresponding [From...] attribute (see [Minimal Apis Parameter Binding](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis/parameter-binding?view=aspnetcore-8.0) for more information).

The following sample demonstrates route and body parameter binding.

``` csharp
public record UpdateBookRequest(Guid Id, [FromBody] UpdateBookRequestBody Body);

public record UpdateBookRequestBody(string Title, string Author, decimal Price);

public record UpdateBookResponse(Guid Id, string Title, string Author, decimal Price);

internal class UpdateBookRequestValidator : AbstractValidator<UpdateBookRequest>
{
  public UpdateBookRequestValidator()
  {
    RuleFor(x => x.Id).NotEmpty();
    RuleFor(x => x.Body.Title).NotEmpty();
    RuleFor(x => x.Body.Author).NotEmpty();
    RuleFor(x => x.Body.Price).GreaterThan(0);
  }
}

internal class UpdateBook(ServiceDbContext db)
  : WebResultEndpoint<UpdateBookRequest, UpdateBookResponse>
{
  protected override void Configure(
    IServiceProvider serviceProvider,
    IRouteGroupConfigurator? parentRouteGroup)
  {
    MapPut("/books/{Id}")
      .Produces<UpdateBookResponse>();
  }

  protected override async Task<Result<UpdateBookResponse>> HandleAsync(
    UpdateBookRequest req,
    CancellationToken ct)
  {
    var entity = await db.Books.FirstOrDefaultAsync(b => b.Id == req.Id, ct);

    if (entity is null)
    {
      return Result<UpdateBookResponse>.NotFound();
    }

    entity.Title = req.Body.Title;
    entity.Author = req.Body.Author;
    entity.Price = req.Body.Price;

    var updated = await db.SaveChangesAsync(ct);
    return updated > 0 ?
      Result.Ok(new UpdateBookResponse(
      Id: req.Id,
      Title: req.Body.Title,
      Author: req.Body.Author,
      Price: req.Body.Price))
      : Result<UpdateBookResponse>.NotFound();
  }
}
```

The following sample demonstrates route and form parameter binding.

```csharp
public record UploadBookRequest(string Title, [FromForm] string Author, IFormFile BookFile);

public record UploadBookResponse(string FileName, long FileSize);

internal class UploadBookRequestValidator : AbstractValidator<UploadBookRequest>
{
  public UploadBookRequestValidator()
  {
    RuleFor(x => x.Title).NotEmpty();
    RuleFor(x => x.Author).NotEmpty();
    RuleFor(x => x.BookFile).NotEmpty();
  }
}

internal class UploadBook
  : WebResultEndpoint<UploadBookRequest, UploadBookResponse>
{
  protected override void Configure(
    IServiceProvider serviceProvider,
    IRouteGroupConfigurator? parentRouteGroup)
  {
    MapPost("/books/upload/{Title}")
      .DisableAntiforgery()
      .Produces<UploadBookResponse>();
  }

  protected override Task<Result<UploadBookResponse>> HandleAsync(
    UploadBookRequest req,
    CancellationToken ct)
  {
    return Task.FromResult(Result.Ok(new UploadBookResponse(
      req.BookFile.FileName,
      req.BookFile.Length)));
  }
}
```

### Route groups

By default, all endpoints are mapped under root route group. It is possible to define route groups similar to using 'MapGroup' extension method used to map Minimal Apis under a group. Since endpoints are configured by endpoint basis in the 'Configure' method of each endpoint, the approach is a little different than regular Minimal Apis, but these are still Minimal Api route groups and can be configured by any extension method of RouteGroupBuilder. Route groups are also subject to auto discovery and registration, similar to endpoints.

- [Create a route group implementation](./samples/ShowcaseWebApi/Features/FeaturesRouteGroup.cs) by inheriting RouteGroupConfigurator and implementing 'Configure' method,
- Configuration of each route group implementation starts with calling MapGroup method with a route pattern prefix. The return of 'MapGroup' method, a RouteGroupBuilder instance, can be used to further customize the route group like a regular Minimal Api route group.
- Apply MapToGroup attribute to either other [route group](./samples/ShowcaseWebApi/Features/Books/Configuration/BooksV1RouteGroup.cs) or [endpoint](./samples/ShowcaseWebApi/Features/Books/CreateBook.cs) classes that will be mapped under created route group. Use type of the new route group implementation as GroupType parameter to the attribute.

Following sample creates a parent route group (FeaturesRouteGroup), a child route group under it (BooksV1RouteGroup) and maps an endpoint (CreateBook) to child route group. Group configuration methods used for this particular sample are all part of Minimal Apis ecosystem and are under [Asp.Versioning](https://github.com/dotnet/aspnet-api-versioning).

```csharp
internal class FeaturesRouteGroup : RouteGroupConfigurator
{
  protected override void Configure(
    IServiceProvider serviceProvider,
    IRouteGroupConfigurator? parentRouteGroup)
  {
    var builder = MapGroup("/api/v{version:apiVersion}");
    var apiVersionSet = builder.NewApiVersionSet()
      .HasApiVersion(new ApiVersion(1))
      .HasApiVersion(new ApiVersion(2))
      .ReportApiVersions()
      .Build();
    builder.WithApiVersionSet(apiVersionSet);
  }
}

[MapToGroup<FeaturesRouteGroup>()]
internal class BooksV1RouteGroup : RouteGroupConfigurator
{
  protected override void Configure(
    IServiceProvider serviceProvider,
    IRouteGroupConfigurator? parentRouteGroup)
  {
    MapGroup("/books")
      .MapToApiVersion(1)
      .WithTags("/BooksV1");
  }
}

[MapToGroup<BooksV1RouteGroup>()]
internal class CreateBook(ServiceDbContext db, ILocationStore location)
  : WebResultEndpoint<CreateBookRequest, CreateBookResponse>
{
  protected override void Configure(
    IServiceProvider serviceProvider,
    IRouteGroupConfigurator? parentRouteGroup)
  {
    MapPost("/")
      .Produces<CreateBookResponse>(StatusCodes.Status201Created);
  }
  protected override async Task<Result<CreateBookResponse>> HandleAsync(
    CreateBookRequest req,
    CancellationToken ct)
  {
    //Handle...
  }
}
```

### Disabling components

DoNotRegister attribute can be used to prevent targeted route group (including its children) or endpoint to be registered during server application startup.

Also prevents targeted service endpoint request to be registered during service endpoint client application startup.

```csharp
[MapToGroup<CustomersV1RouteGroup>()]
[DoNotRegister]
internal class DisabledCustomerFeature
  : MinimalEndpoint<IResult>
{
  protected override void Configure(
    IServiceProvider serviceProvider,
    IRouteGroupConfigurator? parentRouteGroup)
  {
    MapGet("/disabled/");
  }

  protected override Task<IResult> HandleAsync(
    CancellationToken ct)
  {
    throw new NotImplementedException();
  }
}
```

## Samples

[ShowcaseWebApi](https://github.com/modabas/ModEndpoints/tree/main/samples/ShowcaseWebApi) project demonstrates various kinds of endpoint implementations and configurations:
 - MinimalEnpoints samples are in [Customers](https://github.com/modabas/ModEndpoints/tree/main/samples/ShowcaseWebApi/Features/Customers) subfolder,
 - WebResultEndpoints samples are in [Books](https://github.com/modabas/ModEndpoints/tree/main/samples/ShowcaseWebApi/Features/Books) subfolder,
 - BusinessResultEndpoints samples are in [Stores](https://github.com/modabas/ModEndpoints/tree/main/samples/ShowcaseWebApi/Features/Stores) subfolder,
 - ServiceEndpoints samples are in [StoresWithServiceEndpoint](https://github.com/modabas/ModEndpoints/tree/main/samples/ShowcaseWebApi/Features/StoresWithServiceEndpoint) subfolder.

[ServiceEndpointClient](https://github.com/modabas/ModEndpoints/tree/main/samples/ServiceEndpointClient) project demonstrates how to consume ServiceEndpoints.

## Performance

WebResultEndpoints have a slight overhead (3-4%) over regular Minimal Apis on request/sec metric under load tests with 100 virtual users.

MinimalEndpoints perform about same as regular Minimal Apis.

The web apis called for tests, perform only in-process operations like resolving dependency, validating input, calling local methods with no network or disk I/O.

See [test results](./samples/BenchmarkWebApi/BenchmarkFiles/inprocess_benchmark_results.txt) under [BenchmarkFiles](https://github.com/modabas/ModEndpoints/tree/main/samples/BenchmarkWebApi/BenchmarkFiles) folder of BenchmarkWebApi project for detailed results and test scripts.

## Endpoint Types

WebResultEndpoint, BusinessResultEndpoint and ServiceEndpoint, have a 'HandleAsync' method which returns a strongly typed [business result](https://github.com/modabas/ModResults). But they differ in converting these business results into HTTP responses before sending response to client.

MinimalEndpoint within ModEndpoints.Core package, is closest to barebones Minimal Api. Its 'HandleAsync' method support the following types of return values:

- string
- T (Any other type)
- Minimal Api IResult based (Including TypedResults with Results<TResult1, TResultN> return value)

See (How to create responses in Minimal API apps)[https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis/responses?view=aspnetcore-8.0] for detailed information. Other features described previously are common for all of them.

Each type of endpoint has various implementations that accept a request model or not, that has a response model or not.

### MinimalEndpoint

A MinimalEndpoint implementation, after handling request, returns the response model.

- MinimalEndpoint&lt;TRequest, TResponse&gt;: Has a request model, supports request validation and returns a response model.
- MinimalEndpoint&lt;TResponse&gt;: Doesn't have a request model and returns a response model.

### WebResultEndpoint

A WebResultEndpoint implementation, after handling request, maps the [business result](https://github.com/modabas/ModResults) of HandleAsync method to a Minimal Api IResult depending on the business result type, state and failure type (if any). Mapping behaviour can be modified or replaced with a custom one.

- WebResultEndpoint&lt;TRequest, TResponse&gt;: Has a request model, supports request validation and returns a response model as body of Minimal Api IResult if successful.
- WebResultEndpoint&lt;TRequest&gt;: Has a request model, supports request validation, doesn't have a response model to return within Minimal Api IResult.
- WebResultEndpointWithEmptyRequest&lt;TResponse&gt;: Doesn't have a request model and returns a response model as body of Minimal Api IResult if successful.
- WebResultEndpointWithEmptyRequest: Doesn't have a request model, doesn't have a response model to return within Minimal Api IResult.

When result returned from handler method is in Ok state, default WebResultEndpoint response mapping behaviour is:
- For an [endpoint without a response model](./samples/ShowcaseWebApi/Features/Books/DeleteBook.cs), return HTTP 204 No Content.
- For an endpoint with a response model, return HTTP 200 OK with response model as body.

Response HTTP success status code can be configured by [calling 'Produces' extension method during configuration](./samples/ShowcaseWebApi/Features/Books/CreateBook.cs) of endpoint with one of the following status codes:
- StatusCodes.Status200OK,
- StatusCodes.Status201Created,
- StatusCodes.Status202Accepted,
- StatusCodes.Status204NoContent,
- StatusCodes.Status205ResetContent

When result returned from handler method is in Failed state, default WebResultEndpoint response mapping will create a Minimal Api IResult with a 4XX or 5XX HTTP Status Code depending on the FailureType of [business result](https://github.com/modabas/ModResults).

It is also possible to implement a custom response mapping behaviour for a WebResultEndpoint. To do so:
- Create an IResultToResponseMapper implementation,
- Add it to dependency injection service collection with a string key during app startup,
- Apply ResultToResponseMapper attribute to endpoint classes that will be using custom mapper. Use service registration string key as Name property of attribute.

### BusinessResultEndpoint

A BusinessResultEndpoint implementation, after handling request, encapsulates the [business result](https://github.com/modabas/ModResults) of HandleAsync method in a HTTP 200 Minimal Api IResult and sends to client. The [business result](https://github.com/modabas/ModResults) returned may be in Ok or Failed state. This behaviour makes BusinessResultEndpoints more suitable for internal services, where clients are aware of Result or Result&lt;TValue&gt; implementations.

- BusinessResultEndpoint&lt;TRequest, TResultValue&gt;: Has a request model, supports request validation and returns a [Result&lt;TResultValue&gt;](https://github.com/modabas/ModResults) within HTTP 200 IResult.
- BusinessResultEndpoint&lt;TRequest&gt;: Has a request model, supports request validation and returns a [Result](https://github.com/modabas/ModResults) within HTTP 200 IResult.
- BusinessResultEndpointWithEmptyRequest&lt;TResultValue&gt;: Doesn't have a request model and returns a [Result&lt;TResultValue&gt;](https://github.com/modabas/ModResults) within HTTP 200 IResult.
- BusinessResultEndpointWithEmptyRequest: Doesn't have a request model and returns a [Result](https://github.com/modabas/ModResults) within HTTP 200 IResult.


### ServiceEndpoint

This is a very specialized endpoint suitable for internal services. A ServiceEndpoint implementation, similar to BusinessResultEntpoint, encapsulates the response [business result](https://github.com/modabas/ModResults) of HandleAsync method in a HTTP 200 Minimal Api IResult and sends to client. The [business result](https://github.com/modabas/ModResults) returned may be in Ok or Failed state.

- ServiceEndpoint&lt;TRequest, TResultValue&gt;: Has a request model, supports request validation and returns a [Result&lt;TResultValue&gt;](https://github.com/modabas/ModResults) within HTTP 200 IResult.
- ServiceEndpoint&lt;TRequest&gt;: Has a request model, supports request validation and returns a [Result](https://github.com/modabas/ModResults) within HTTP 200 IResult.

A ServiceEndpoint has following special traits and constraints:
- A ServiceEndpoint is always registered as a POST method, and its bound pattern is determined accourding to its request type.
- Request model defined for a ServiceEndpoint is bound with [FromBody] attribute.
- A ServiceEndpoint's request must implement either IServiceRequest (for endpoints implementing ServiceEndpoint&lt;TRequest&gt;) or IServiceRequest&lt;TResultValue&gt; (for endpoints implementing ServiceEndpoint&lt;TRequest, TResultValue&gt;)
- A ServiceEndpoint's request is specific to that endpoint. Each endpoint must have its unique request type.
- To utilize the advantages of a ServiceEndpoint over other endpoint types, its request and response types has to be shared with clients and therefore has to be in a seperate class library.

These enable clients to call ServiceEndpoints by a specialized message channel resolved from dependency injection, which has to be registered at client application startup with only service base address and service request type information. No other knowledge about service or client implementation is required.

Have a look at [sample ServiceEndpoint implementations](https://github.com/modabas/ModEndpoints/tree/main/samples/ShowcaseWebApi/Features/StoresWithServiceEndpoint) along with [sample client implementation](https://github.com/modabas/ModEndpoints/tree/main/samples/Client) and [request/response model shared library](https://github.com/modabas/ModEndpoints/tree/main/samples/ShowcaseWebApi.FeatureContracts).

### ServiceEndpoint clients

A client application consuming ServiceEndpoints, creates service channel registry entries for those endpoints (remote services) during application startup, basically mapping which request message type will be sent by using which HttpClient configuration (base address, timeout, handlers, resilience definitions, etc.). Service channel utilizes IHttpClientFactory and HttpClient underneath and is configured similarly.

Registration can be done either for all service requests in an assembly...
```csharp
var baseAddress = "https://...";
var clientName = "MyClient";
builder.Services.AddRemoteServicesWithNewClient(
  typeof(ListStoresRequest).Assembly,
  clientName,
  (sp, client) =>
  {
    client.BaseAddress = new Uri(baseAddress);
    client.Timeout = TimeSpan.FromSeconds(5);
  },
  clientBuilder => clientBuilder.AddTransientHttpErrorPolicy(
    policyBuilder => policyBuilder.CircuitBreakerAsync(5, TimeSpan.FromSeconds(30))));
```

or alternatively, remote services can be registered one by one, adding each service request individually...
```csharp
var baseAddress = "https://...";
var clientName = "MyClient";
builder.Services.AddRemoteServiceWithNewClient<ListStoresRequest>(clientName,
  (sp, client) =>
  {
    client.BaseAddress = new Uri(baseAddress);
    client.Timeout = TimeSpan.FromSeconds(5);
  },
  clientBuilder => clientBuilder.AddTransientHttpErrorPolicy(
    policyBuilder => policyBuilder.CircuitBreakerAsync(5, TimeSpan.FromSeconds(30))));
builder.Services.AddRemoteServiceToExistingClient<GetStoreByIdRequest>(clientName);
builder.Services.AddRemoteServiceToExistingClient<DeleteStoreRequest>(clientName);
builder.Services.AddRemoteServiceToExistingClient<CreateStoreRequest>(clientName);
builder.Services.AddRemoteServiceToExistingClient<UpdateStoreRequest>(clientName);
```

Then remote services are called with IServiceChannel instance resolved from DI...
```csharp
  using IServiceScope serviceScope = hostProvider.CreateScope();
  IServiceProvider provider = serviceScope.ServiceProvider;

  //resolve service channel from DI
  var channel = provider.GetRequiredService<IServiceChannel>();
  //send request over channel to remote service
  var listResult = await channel.SendAsync<ListStoresRequest, ListStoresResponse>(new ListStoresRequest(), ct);

```