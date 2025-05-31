# ModEndpoints

[![Nuget downloads](https://img.shields.io/nuget/v/ModEndpoints.svg)](https://www.nuget.org/packages/ModEndpoints/)
[![Nuget](https://img.shields.io/nuget/dt/ModEndpoints)](https://www.nuget.org/packages/ModEndpoints/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://github.com/modabas/ModEndpoints/blob/main/LICENSE.txt)

**ModEndpoints** provides a structured approach to organizing ASP.NET Core Minimal APIs using the REPR (Request - Endpoint - Response) pattern. It enhances the Minimal API paradigm by introducing reusable, testable, and consistent components for request validation, handling, and response mapping.

---

## ✨ Features

- **REPR Pattern Implementation**: Organizes Minimal APIs into Request, Endpoint and Response components.
- **Seamless Integration**: Fully compatible with ASP.NET Core Minimal APIs, supporting configurations, parameter binding, authentication, OpenAPI tooling, filters, and more.
- **Auto-Discovery and Registration**: Automatically discovers and registers endpoints.
- **FluentValidation Support**: Built-in validation using FluentValidation; requests are automatically validated if a validator is registered.
- **Dependency Injection**: Supports constructor-based dependency injection in endpoint implementations.
- **Type-Safe Responses**: Enforces response model type safety in request handlers.

---

## 🧩 Endpoint Types

### MinimalEndpoint  

- **Purpose**: Enables full flexibility and capability of Minimal APIs within a structured approach.  
- **Usage**: Suitable for implementing any Minimal API endpoint, from simple to complex scenarios, while benefiting from clear separation of concerns and all features available to Minimal APIs.
- **Package**: `ModEndpoints.Core`

### WebResultEndpoint

- **Purpose**: Converts business results into standardized HTTP status codes and response formats, ensuring consistent and type-safe API behavior.
- **Usage**: Perfect for centralizing and abstracting the logic of converting business results into HTTP responses.
- **Package**: `ModEndpoints`

### BusinessResultEndpoint  

- **Purpose**: Returns raw business results directly within an HTTP 200 OK response without additional formatting.  
- **Usage**: Ideal for internal API layers or scenarios where the raw business result is sufficient for the client.
- **Package**: `ModEndpoints`

### ServiceEndpoint

- **Purpose**: Designed for simplifying remote service consumption with strongly typed request and response models.
- **Usage**: Works in conjunction with the `ModEndpoints.RemoteServices` package to abstract HTTP plumbing on the client side.
- **Package**: `ModEndpoints`

> **Note**: For detailed information on each endpoint type, refer to the [Endpoint Types](./docs/EndpointTypes.md) documentation.

---

## ⚙️ Workflow

Each endpoint must implement two virtual methods:

1. **Configure**: Invoked during application startup to define the route and HTTP method for an endpoint. It begins with calling input parameter `builder`'s methods like `MapGet`, `MapPost`, etc., to specify the route pattern. The returned `RouteHandlerBuilder` from the Map[HttpVerb] method can then be used for further endpoint customization.

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

### Install the NuGet Package:

```bash
dotnet add package ModEndpoints
```

> **Note**: To use only MinimalEndpoints, you can install the `ModEndpoints.Core` package.

### Register Endpoints:

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

A `MinimalEndpoint` is the most straighforward way to define a Minimal API in REPR format.

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
    EndpointConfigurationBuilder builder,
    ConfigurationContext<EndpointConfigurationParameters> configurationContext)
  {
    builder.MapGet("MinimalEndpoints/HelloWorld/{Name}")
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
    EndpointConfigurationBuilder builder,
    ConfigurationContext<EndpointConfigurationParameters> configurationContext)
  {
    builder.MapGet("/weatherforecast")
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

A `WebResultEndpoint` can be utilized to abstract the logic for converting business results into HTTP responses of endpoints. Configuration and request handling is similar to MinimalEndpoint, but a WebResultEndpoint handler method also has the benefit of having a strongly typed return while having potential to return different HTTP response codes according to business result state.

This sample demonstrates a GET endpoint with basic configuration and without any request model binding. Business result instance returned from handler method is converted to a Minimal API IResult based response by WebResultEndpoint before being sent to client.

``` csharp
public record ListBooksResponse(List<ListBooksResponseItem> Books);

public record ListBooksResponseItem(Guid Id, string Title, string Author, decimal Price);

internal class ListBooks(ServiceDbContext db)
  : WebResultEndpointWithEmptyRequest<ListBooksResponse>
{
  protected override void Configure(
    EndpointConfigurationBuilder builder,
    ConfigurationContext<EndpointConfigurationParameters> configurationContext)
  {
    builder.MapGet("/books")
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

### Further Reading  

For more examples, refer to the following:  

- [Parameter Binding](./docs/ParameterBinding.md)  
- [Request Validation](./docs/RequestValidation.md)
- [Route Groups](./docs/RouteGroups.md)
- [Disabling Components](./docs/DisablingComponents.md)
- [Open API Documentation](./docs/OpenApiDocumentation.md)
- [Handling Files](./docs/HandlingFiles.md)
- [Endpoint Types](./docs/EndpointTypes.md)
- [Result Pattern Integration](./docs/ResultPatternIntegration.md)
- [IAsyncEnumerable Response](./docs/IAsyncEnumerableResponse.md)
- [WebResultEndpoint Response Mapping](./docs/WebResultEndpointResponseMapping.md)
- [ServiceEndpoint Clients](./docs/ServiceEndpointClients.md)

---

## 📚 Samples

[ShowcaseWebApi](./samples/ShowcaseWebApi) project demonstrates all endpoint types in action and also API documentation with Swagger and Swashbuckle:
 - `MinimalEnpoint` samples are in [Customers](./samples/ShowcaseWebApi/Features/Customers) subfolder,
 - `WebResultEndpoint` samples are in [Books](./samples/ShowcaseWebApi/Features/Books) subfolder,
 - `BusinessResultEndpoint` samples are in [Stores](./samples/ShowcaseWebApi/Features/Stores) subfolder,
 - `ServiceEndpoint` samples are in [StoresWithServiceEndpoint](./samples/ShowcaseWebApi/Features/StoresWithServiceEndpoint) subfolder.

[ServiceEndpointClient](./samples/ServiceEndpointClient) project demonstrates how to consume ServiceEndpoints.

[WeatherForecastWebApi](./samples/WeatherForecastWebApi) project demonstrates how GetWeatherForecast Minimal API in ASP.NET Core Web API project template can be written using MinimalEndpoints. Also demostrates API documentation with Scalar and OpenAPI.

---

## 📊 Performance

Under load tests with 100 virtual users:  
- MinimalEndpoints perform nearly the same (~1-2%) as Minimal APIs,
- WebResultEndpoints introduce a slight overhead (~2-3%) compared to Minimal APIs in terms of requests per second.

The web apis called for tests, perform only in-process operations like resolving dependency, validating input, calling local methods with no network or disk I/O.

See [test results](./samples/BenchmarkWebApi/BenchmarkFiles/Results/1.3.1/inprocess_benchmark_results.txt) under [BenchmarkFiles](https://github.com/modabas/ModEndpoints/tree/main/samples/BenchmarkWebApi/BenchmarkFiles) folder of BenchmarkWebApi project for detailed results and test scripts.

---
