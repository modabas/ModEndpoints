# ModEndpoints

WebResultEndpoints and ServiceResultEndpoints organize ASP.NET Core Minimal Apis in REPR format endpoints and are integrated with [result](https://github.com/modabas/ModResults) pattern out of box.

# ModEndpoints.Core

MinimalEndpoints is the barebone implementation for organizing ASP.NET Core Minimal Apis in REPR format endpoints. Does not come integrated with a result pattern like endpoints in ModEndpoints project.

Also contains core classes for ModEndpoints project.

## Introduction

The WebResultEndpoint and ServiceResultEndpoint abstractions are a structured approach to defining endpoints in ASP.NET Core applications. It extends the Minimal Api pattern with reusable, testable, and consistent components for request handling, validation, and response mapping.

## Key Features

 - Encapsulates endpoint behaviors like request validation, request handling, and response mapping.
 - Separates concerns to promote clean, maintainable code.
 - Uses the Minimal Apis and routing system provided by ASP.NET Core. Configuration, parameter binding, authentication, Open Api tooling, filters, etc. are all Minimal Apis under the hood. Supports anything that Minimal Apis does.
 - Abstracts the logic for converting business results into HTTP responses.
 - Supports auto discovery and registration.
 - Has built-in validation support with [FluentValidation](https://github.com/FluentValidation/FluentValidation). If a validator is registered for request model, it is automatically validated before being handled.
 - Supports constructor dependency injection in endpoint implementations.

## Performance

WebResultEndpoints have a slight overhead (3-4%) over regular Minimal Apis on request/sec metric under load tests with 100 virtual users.

MinimalEndpoints perform about same as regular Minimal Apis.

The web apis called for tests, perform only in-process operations like resolving dependency, validating input, calling local methods with no network or disk I/O.

See [test results](./samples/BenchmarkWebApi/BenchmarkFiles/inprocess_benchmark_results.txt) under [BenchmarkFiles](https://github.com/modabas/ModEndpoints/tree/main/samples/BenchmarkWebApi/BenchmarkFiles) folder of BenchmarkWebApi project for detailed results and test scripts.

## Workflow

An endpoint must implement two virtual methods: Configure and HandleAsync.

### Configuration:

The 'Configure' method is called at application startup to define routes and associate them with handler methods (MapGet, MapPost, etc.).

### Request Handling:

The request is processed in 'HandleAsync' method which returns a [result](https://github.com/modabas/ModResults). This result is handled differently for each endpoint type before being sent to client.

## Endpoint Types

WebResultEndpoint and ServiceResultEndpoint, the two abstract endpoint bases, differ only in converting business results into HTTP responses before sending response to client. Other features described previously are common for both of them.

### WebResultEndpoint

A WebResultEndpoint implementation, after handling request, maps the [result](https://github.com/modabas/ModResults) of HandleAsync method to an IResult depending on the result type, state and failure type (if any). Mapping behaviour can be modified or replaced with a custom one.

### ServiceResultEndpoint

A ServiceResultEndpoint implementation, after handling request, encapsulates the [result](https://github.com/modabas/ModResults) of HandleAsync method in a HTTP 200 IResult (Minimal Api response type) and sends to client. This behaviour makes ServiceResultEndpoints more suitable for internal services, where clients are aware of Result or Result&lt;TValue&gt; implementations.

## Service Registration

Use AddModEndpointsFromAssembly extension method to register all endpoints defined in an assembly.

Use MapModEndpoint extension method to map registered endpoints.

These methods register and map services required for all endpoint types.

 ``` csharp
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddModEndpointsFromAssembly(typeof(Program).Assembly);

var app = builder.Build();

app.MapModEndpoints();

app.Run();
```

## Usage

Configuration of each endpoint implementation starts with calling one of the MapGet, MapPost, MapPut, MapDelete and MapPatch methods with a route pattern string. The return from any of these methods, a RouteHandlerBuilder instance, can be used to further customize the endpoint like a regular Minimal Api.

Have a look at [ShowcaseWebApi](https://github.com/modabas/ModEndpoints/tree/main/samples/ShowcaseWebApi) project for various kinds of endpoint implementations and configurations.

### A basic sample: An endpoint with empty request and configured for GET

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
    var entity = await db.Books.FindAsync([req.Id], ct);

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

### Response mapping for WebResultEndpoint

Default mapping behaviour is:
- For an [endpoint without a response model](./samples/ShowcaseWebApi/Features/Books/DeleteBook.cs), return HTTP 204 No Content.
- For an endpoint with a response model, return HTTP 200 OK with response model as body.

For both cases, response HTTP status code can be changed by [calling 'Produces' extension method during configuration](./samples/ShowcaseWebApi/Features/Books/CreateBook.cs) with one of the following status codes:
- StatusCodes.Status200OK,
- StatusCodes.Status201Created,
- StatusCodes.Status202Accepted,
- StatusCodes.Status204NoContent,
- StatusCodes.Status205ResetContent

For implementing custom response mapping for an endpoint:
- Create an IResultToResponseMapper implementation,
- Add it to dependency injection service collection with a string key during app startup,
- Apply ResultToResponseMapper attribute to endpoint classes that will be using custom mapper. Use service registration string key as Name property of attribute.

### Route groups

By default, all endpoints are mapped under root route group. It is possible to define route groups similar to using 'MapGroup' extension method and to map Minimal Apis under said group. Since endpoints are configured by endpoint basis in the 'Configure' method of each endpoint, the approach is a little different than regular Minimal Apis, but these are still Minimal Api route groups and can be configured by any extension method of RouteGroupConfigurator. Route groups are also subject to auto discovery and registration like endpoints.

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