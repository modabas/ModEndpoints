# ModEndpoints

[![Nuget downloads](https://img.shields.io/nuget/v/ModEndpoints.svg)](https://www.nuget.org/packages/ModEndpoints/)
[![Nuget](https://img.shields.io/nuget/dt/ModEndpoints)](https://www.nuget.org/packages/ModEndpoints/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://github.com/modabas/ModEndpoints/blob/main/LICENSE.txt)

**ModEndpoints** provides a structured approach to organizing ASP.NET Core Minimal APIs using the REPR (Request, Endpoint, Processor, Response) pattern. It enhances the Minimal API paradigm by introducing reusable, testable, and consistent components for request validation, handling, and response mapping.

---

## ✨ Features

- **REPR Pattern Implementation**: Organizes Minimal APIs into Request, Endpoint, Processor, and Response components.
- **Seamless Integration**: Fully compatible with ASP.NET Core Minimal APIs, supporting configurations, parameter binding, authentication, OpenAPI tooling, filters, and more.
- **Auto-Discovery and Registration**: Automatically discovers and registers endpoints.
- **FluentValidation Support**: Built-in validation using FluentValidation; requests are automatically validated if a validator is registered.
- **Dependency Injection**: Supports constructor-based dependency injection in endpoint implementations.
- **Type-Safe Responses**: Enforces response model type safety in request handlers.

---

## 🧩 Endpoint Types

### MinimalEndpoint

- **Purpose**: The barebone implementation for organizing ASP.NET Core Minimal APIs in REPR format endpoints.
- **Usage**: Ideal for straightforward Minimal API implementations without additional processing.

### WebResultEndpoint

- **Purpose**: Transforms business results into HTTP responses.
- **Usage**: Ideal for endpoints that need to return `IResult` types to the client.

### BusinessResultEndpoint

- **Purpose**: Returns raw business results without HTTP transformation.
- **Usage**: Suitable for internal API layers where HTTP response formatting is unnecessary.

### ServiceEndpoint

- **Purpose**: Designed for remote service consumption with strongly typed request and response models.
- **Usage**: Works in conjunction with the `ModEndpoints.RemoteServices` package to abstract HTTP plumbing on the client side.

> **Note**: For detailed information on each endpoint type, refer to the [Endpoint Types](https://github.com/modabas/ModEndpoints/tree/main/docs/EndpointTypes.md) documentation.

---

## ⚙️ Workflow

Each endpoint must implement two virtual methods:

1. **Configure**: Called at application startup to define routes and associate them with handler methods (`MapGet`, `MapPost`, etc.). The returned `RouteHandlerBuilder` can be used to further customize endpoints.

2. **HandleAsync**: Contains the logic to handle incoming requests. Called after the request is validated (if applicable).

> **Note**: `ServiceEndpoint` provides a default implementation for the `Configure` method, which can be overridden if necessary.

---

## 📦 Packages

- **ModEndpoints.Core**: Provides `MinimalEndpoint` structure and contains the base implementations for organizing Minimal APIs in the REPR format.
- **ModEndpoints**: Provides `WebResultEndpoint`, `BusinessResultEndpoint`, and `ServiceEndpoint` structures.
- **ModEndpoints.RemoteServices**: Offers client implementations for `ServiceEndpoint` to facilitate remote service consumption.
- **ModEndpoints.RemoteServices.Core**: Contains interfaces required for `ServiceEndpoint` request models.

---

## 🛠️ Getting Started

### **Install the NuGet Package**:

```bash
dotnet add package ModEndpoints
```

> **Note**: To use only MinimalEndpoints, you can install the `ModEndpoints.Core` package.

### **Register Endpoints**:

In your `Program.cs`:

``` csharp
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddModEndpointsFromAssemblyContaining<MyEndpoint>();
//Validation
builder.Services.AddValidatorsFromAssemblyContaining<MyValidator>(includeInternalTypes: true);

var app = builder.Build();

app.MapModEndpoints();

app.Run();
```

> **Note**: If you are using the `ModEndpoints.Core` package, replace `AddModEndpointsFromAssemblyContaining` with `AddModEndpointsCoreFromAssemblyContaining` and `MapModEndpoints` with `MapModEndpointsCore`.

### Define a Minimal API in REPR format

A [MinimalEndpoint](#minimalendpoint) is the most straighforward way to define a Minimal API in REPR format.

Configuration of each endpoint implementation starts with calling one of the MapGet, MapPost, MapPut, MapDelete and MapPatch methods with a route pattern string. The return from any of these methods, a RouteHandlerBuilder instance, can be used to further customize the endpoint similar to a Minimal API.

The request is processed in 'HandleAsync' method. Request is passed to handler method as parameter after validation (if a validator is registered for request model). Handler method returns a response model or a string or a Minimal API IResult based response.

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

The GetWeatherForecast example from the .NET Core Web API project template can be rewritten using MinimalEndpoints in the REPR format as shown below:
``` csharp
internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
  public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}

internal class GetWeatherForecast : MinimalEndpoint<WeatherForecast[]>
{
  private static readonly string[] _summaries =
  [
    "Freezing",
    "Bracing",
    "Chilly",
    "Cool",
    "Mild",
    "Warm",
    "Balmy",
    "Hot",
    "Sweltering",
    "Scorching"
  ];

  protected override void Configure(
    IServiceProvider serviceProvider,
    IRouteGroupConfigurator? parentRouteGroup)
  {
    MapGet("/weatherforecast")
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
```

### Integration with result pattern: A GET WebResultEndpoint with empty request

A [WebResultEndpoint](#webresultendpoint) can be utilized to abstract the logic for converting business results into HTTP responses of endpoints. Configuration and request handling is similar to MinimalEndpoint, but a WebResultEndpoint handler method also has the benefit of having a strongly typed return while having potential to return different HTTP response codes according to business result state.

This sample demonstrates a GET endpoint with basic configuration and without any request model binding. Business result instance returned from handler method is converted to a Minimal API IResult based response by WebResultEndpoint before being sent to client.

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

Request model defined for an endpoint is bound with [AsParameters] attribute (except for ServiceEndpoints). Any field under request model can be bound from route, query, body, form, etc. with corresponding [From...] attribute (see [Minimal APIs Parameter Binding](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis/parameter-binding?view=aspnetcore-8.0) for more information).

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

By default, all endpoints are mapped under root route group. It is possible to define route groups similar to using 'MapGroup' extension method used to map Minimal APIs under a group. Since endpoints are configured by endpoint basis in the 'Configure' method of each endpoint, the approach is a little different than configuring a Minimal API. But route group configuration still utilize Minimal API route groups and can be decorated by any extension method of RouteGroupBuilder. Route groups are also subject to auto discovery and registration, similar to endpoints.

- [Create a route group implementation](./samples/ShowcaseWebApi/Features/FeaturesRouteGroup.cs) by inheriting RouteGroupConfigurator and implementing 'Configure' method,
- Configuration of each route group implementation starts with calling MapGroup method with a route pattern prefix. The return of 'MapGroup' method, a RouteGroupBuilder instance, can be used to further customize the route group like any Minimal API route group.
- Apply MapToGroup attribute to either other [route group](./samples/ShowcaseWebApi/Features/Books/Configuration/BooksV1RouteGroup.cs) or [endpoint](./samples/ShowcaseWebApi/Features/Books/CreateBook.cs) classes that will be mapped under created route group. Use type of the new route group implementation as GroupType parameter to the attribute.

Following sample creates a parent route group (FeaturesRouteGroup), a child route group under it (BooksV1RouteGroup) and maps an endpoint (CreateBook) to child route group. Group configuration methods used for this particular sample are all part of Minimal APIs ecosystem and are under [Asp.Versioning](https://github.com/dotnet/aspnet-api-versioning).

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

---

## 📚 Samples

[ShowcaseWebApi](https://github.com/modabas/ModEndpoints/tree/main/samples/ShowcaseWebApi) project demonstrates all endpoint types in action:
 - `MinimalEnpoint` samples are in [Customers](https://github.com/modabas/ModEndpoints/tree/main/samples/ShowcaseWebApi/Features/Customers) subfolder,
 - `WebResultEndpoint` samples are in [Books](https://github.com/modabas/ModEndpoints/tree/main/samples/ShowcaseWebApi/Features/Books) subfolder,
 - `BusinessResultEndpoint` samples are in [Stores](https://github.com/modabas/ModEndpoints/tree/main/samples/ShowcaseWebApi/Features/Stores) subfolder,
 - `ServiceEndpoint` samples are in [StoresWithServiceEndpoint](https://github.com/modabas/ModEndpoints/tree/main/samples/ShowcaseWebApi/Features/StoresWithServiceEndpoint) subfolder.

[ServiceEndpointClient](https://github.com/modabas/ModEndpoints/tree/main/samples/ServiceEndpointClient) project demonstrates how to consume ServiceEndpoints.

---

## 📊 Performance

Under load tests with 100 virtual users:  
- MinimalEndpoints perform nearly the same (~1-2%) as Minimal APIs,
- WebResultEndpoints introduce a slight overhead (~2-3%) compared to Minimal APIs in terms of requests per second.

The web apis called for tests, perform only in-process operations like resolving dependency, validating input, calling local methods with no network or disk I/O.

See [test results](./samples/BenchmarkWebApi/BenchmarkFiles/Results/1.0.0/inprocess_benchmark_results.txt) under [BenchmarkFiles](https://github.com/modabas/ModEndpoints/tree/main/samples/BenchmarkWebApi/BenchmarkFiles) folder of BenchmarkWebApi project for detailed results and test scripts.

---

