# ModEndpoints

[![Nuget downloads](https://img.shields.io/nuget/v/ModEndpoints.svg)](https://www.nuget.org/packages/ModEndpoints/)
[![Nuget](https://img.shields.io/nuget/dt/ModEndpoints)](https://www.nuget.org/packages/ModEndpoints/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://github.com/modabas/ModEndpoints/blob/main/LICENSE.txt)

WebResultEndpoints and BusinessResultEndpoints organize ASP.NET Core Minimal Apis in REPR format endpoints and are integrated with [result](https://github.com/modabas/ModResults) pattern out of box.

# ModEndpoints.Core

MinimalEndpoints is the barebone implementation for organizing ASP.NET Core Minimal Apis in REPR format endpoints. Does not come integrated with a result pattern like endpoints in ModEndpoints project.

Also contains core classes for ModEndpoints project.

## Introduction

The WebResultEndpoint and BusinessResultEndpoint abstractions are a structured approach to defining endpoints in ASP.NET Core applications. It extends the Minimal Api pattern with reusable, testable, and consistent components for request handling, validation, and response mapping.

## Key Features

 - Organizes ASP.NET Core Minimal Apis in REPR pattern endpoints
 - Encapsulates endpoint behaviors like request validation, request handling, and response mapping*.
 - Supports anything that Minimal Apis does. Configuration, parameter binding, authentication, Open Api tooling, filters, etc. are all Minimal Apis under the hood.
 - Supports auto discovery and registration.
 - Has built-in validation support with [FluentValidation](https://github.com/FluentValidation/FluentValidation). If a validator is registered for request model, request is automatically validated before being handled.
 - Supports constructor dependency injection in endpoint implementations.
 - Enforces response model type safety in request handlers.
 - Abstracts the logic for converting business results into HTTP responses.

 *WebResultEndpoint abstracts the logic for converting business results into HTTP responses.
 
## Workflow

An endpoint must implement two virtual methods: Configure and HandleAsync.

### Configuration:

The 'Configure' method is called at application startup to define routes and associate them with handler methods (MapGet, MapPost, etc.).

### Request Handling:

The request is processed in 'HandleAsync' method which returns a strongly typed [business result](https://github.com/modabas/ModResults). This business result is handled differently for each endpoint type before being sent to client.

## Quickstart

### Service Registration

Use AddModEndpointsFromAssembly extension method to register all endpoints defined in an assembly.

Optional: Use FluentValidation.DependencyInjectionExtensions package to add FluentValidation validators to dependency injection for request validation.

Use MapModEndpoint extension method to map registered endpoints.

These methods register and map services required for all endpoint types.

 ``` csharp
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddModEndpointsFromAssembly(typeof(MyEndpoint).Assembly);
//Validation
builder.Services.AddValidatorsFromAssemblyContaining<MyValidator>(includeInternalTypes: true);

var app = builder.Build();

app.MapModEndpoints();

app.Run();
```

### A basic sample: A GET endpoint with empty request

Configuration of each endpoint implementation starts with calling one of the MapGet, MapPost, MapPut, MapDelete and MapPatch methods with a route pattern string. The return from any of these methods, a RouteHandlerBuilder instance, can be used to further customize the endpoint like a regular Minimal Api.

Have a look at [ShowcaseWebApi](https://github.com/modabas/ModEndpoints/tree/main/samples/ShowcaseWebApi) project for various kinds of endpoint implementations and configurations.

This sample demonstrates a GET endpoint with basic configuration and without any request model binding.

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

Request model defined for an endpoint is bound with [AsParameters] attribute. Any field under request model can be bound from route, query, body, form, etc. with corresponding [From...] attribute (see [Minimal Apis Parameter Binding](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis/parameter-binding?view=aspnetcore-8.0) for more information).

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

By default, all endpoints are mapped under root route group. It is possible to define route groups similar to using 'MapGroup' extension method and to map Minimal Apis under said group. Since endpoints are configured by endpoint basis in the 'Configure' method of each endpoint, the approach is a little different than regular Minimal Apis, but these are still Minimal Api route groups and can be configured by any extension method of RouteGroupConfigurator. Route groups are also subject to auto discovery and registration, similar to endpoints.

- [Create a route group implementation](./samples/ShowcaseWebApi/Features/FeaturesRouteGroup.cs) by inheriting RouteGroupConfigurator and implementing 'Configure' method,
- Configuration of each route group implementation starts with calling MapGroup method with a route pattern prefix. The return of 'MapGroup' method, a RouteGroupBuilder instance, can be used to further customize the route group like a regular Minimal Api route group.
- Apply RouteGroupMember attribute to either other [route group](./samples/ShowcaseWebApi/Features/Books/Configuration/BooksV1RouteGroup.cs) or [endpoint](./samples/ShowcaseWebApi/Features/Books/CreateBook.cs) classes that will be mapped under created route group. Use type of the new route group implementation as ParentGroupType property of attribute.

Following sample creates a parent route group (FeaturesRouteGroup), a child route group (BooksV1RouteGroup) and maps an endpoint (CreateBook) to child route group. Group configuration methods used for this particular sample are all part of Minimal Apis ecosystem and are under [Asp.Versioning](https://github.com/dotnet/aspnet-api-versioning) .

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

[RouteGroupMember(typeof(FeaturesRouteGroup))]
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

[RouteGroupMember(typeof(BooksV1RouteGroup))]
internal class CreateBook(ServiceDbContext db, ILocationStore location)
  : WebResultEndpoint<CreateBookRequest, CreateBookResponse>
{
  protected override void Configure(
    IServiceProvider serviceProvider,
    IRouteGroupConfigurator? parentRouteGroup)
  {
    //Configure...
  }
  protected override async Task<Result<CreateBookResponse>> HandleAsync(
    CreateBookRequest req,
    CancellationToken ct)
  {
    //Handle...
  }
}
```

## Performance

WebResultEndpoints have a slight overhead (3-4%) over regular Minimal Apis on request/sec metric under load tests with 100 virtual users.

MinimalEndpoints perform about same as regular Minimal Apis.

The web apis called for tests, perform only in-process operations like resolving dependency, validating input, calling local methods with no network or disk I/O.

See [test results](./samples/BenchmarkWebApi/BenchmarkFiles/inprocess_benchmark_results.txt) under [BenchmarkFiles](https://github.com/modabas/ModEndpoints/tree/main/samples/BenchmarkWebApi/BenchmarkFiles) folder of BenchmarkWebApi project for detailed results and test scripts.

## Endpoint Types

WebResultEndpoint and BusinessResultEndpoint, the two abstract endpoint bases, have a 'HandleAsync' method which returns a strongly typed [business result](https://github.com/modabas/ModResults).

These two endpoint types differ only in converting these business results into HTTP responses before sending response to client.

MinimalEndpoint within ModEndpoints.Core package, is closest to barebones Minimal Api. Its 'HandleAsync' method support the following types of return values:

- string
- T (Any other type)
- Minimal Api IResult based

Other features described previously are common for all of them.

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
