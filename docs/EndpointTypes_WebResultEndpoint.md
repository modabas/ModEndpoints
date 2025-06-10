# WebResultEndpoint

A WebResultEndpoint implementation, after handling request, maps the [business result](https://github.com/modabas/ModResults) of HandleAsync method to a Minimal API IResult depending on the business result type, state and failure type (if any). Mapping behaviour can be modified or replaced with a custom one.

Request model (if any) defined for a WebResultEndpoint is bound with [AsParameters] attribute.

- **WebResultEndpoint&lt;TRequest, TResponse&gt;**: Has a request model, supports request validation and returns a response model as body of Minimal API IResult if successful.
``` csharp
public record CreateBookRequest([FromBody] CreateBookRequestBody Body);

public record CreateBookRequestBody(string Title, string Author, decimal Price);

public record CreateBookResponse(Guid Id);

internal class CreateBookRequestValidator : AbstractValidator<CreateBookRequest>
{
  public CreateBookRequestValidator()
  {
    RuleFor(x => x.Body.Title).NotEmpty();
    RuleFor(x => x.Body.Author).NotEmpty();
    RuleFor(x => x.Body.Price).GreaterThan(0);
  }
}

internal class CreateBook(ServiceDbContext db, ILocationStore location)
  : WebResultEndpoint<CreateBookRequest, CreateBookResponse>
{
  protected override void Configure(
    EndpointConfigurationBuilder builder,
    ConfigurationContext<EndpointConfigurationParameters> configurationContext)
  {
    builder.MapPost("/")
      .Produces<CreateBookResponse>(StatusCodes.Status201Created);
  }

  protected override async Task<Result<CreateBookResponse>> HandleAsync(
    CreateBookRequest req,
    CancellationToken ct)
  {
    var book = new BookEntity()
    {
      Title = req.Body.Title,
      Author = req.Body.Author,
      Price = req.Body.Price
    };

    db.Books.Add(book);
    await db.SaveChangesAsync(ct);

    await location.SetValueAsync(
      typeof(GetBookById).FullName,
      new { id = book.Id },
      ct);
    return Result.Ok(new CreateBookResponse(book.Id));
  }
}
```

- **WebResultEndpoint&lt;TRequest&gt;**: Has a request model, supports request validation, doesn't have a response model to return within Minimal API IResult.
``` csharp
public record UpdateBookRequest(Guid Id, [FromBody] UpdateBookRequestBody Body);

public record UpdateBookRequestBody(string Title, string Author, decimal Price);

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

internal class UpdateBook
  : WebResultEndpoint<UpdateBookRequest>
{
  protected override void Configure(
    EndpointConfigurationBuilder builder,
    ConfigurationContext<EndpointConfigurationParameters> configurationContext)
  {
    builder.MapPut("/{Id}");
  }

  protected override Task<Result> HandleAsync(
    UpdateBookRequest req,
    CancellationToken ct)
  {
    return Task.FromResult(Result.Ok());
  }
}
```

- **WebResultEndpointWithEmptyRequest&lt;TResponse&gt;**: Doesn't have a request model and returns a response model as body of Minimal API IResult if successful.
``` csharp
public record ListBooksResponse(List<ListBooksResponseItem> Books);
public record ListBooksResponseItem(Guid Id, string Title, string Author, decimal Price);

internal class ListBooks
  : WebResultEndpointWithEmptyRequest<ListBooksResponse>
{
  protected override void Configure(
    EndpointConfigurationBuilder builder,
    ConfigurationContext<EndpointConfigurationParameters> configurationContext)
  {
    builder.MapGet("/")
      .Produces<ListBooksResponse>();
  }

  protected override async Task<Result<ListBooksResponse>> HandleAsync(
    CancellationToken ct)
  {
    await Task.CompletedTask; // Simulate async work

    return new ListBooksResponse(Books:
      [
        new ListBooksResponseItem(Guid.NewGuid(), "Book 1", "Author 1", 19.99m),
        new ListBooksResponseItem(Guid.NewGuid(), "Book 2", "Author 2", 29.99m)
      ]);

  }
}
```

- **WebResultEndpointWithEmptyRequest**: Doesn't have a request model, doesn't have a response model to return within Minimal API IResult.
``` csharp
internal class ResultEndpointWithEmptyRequest
  : WebResultEndpointWithEmptyRequest
{
  protected override void Configure(
    EndpointConfigurationBuilder builder,
    ConfigurationContext<EndpointConfigurationParameters> configurationContext)
  {
    builder.MapDelete("/").Produces(StatusCodes.Status204NoContent);
  }

  protected override async Task<Result> HandleAsync(
    CancellationToken ct)
  {
    await Task.CompletedTask; // Simulate async work
    return Result.Ok();
  }
}
```

When result returned from handler method is in Ok state, default WebResultEndpoint response mapping behaviour is:
- For an [endpoint without a response model](../samples/ShowcaseWebApi/Features/Books/DeleteBook.cs), return HTTP 204 No Content.
- For an endpoint with a response model, return HTTP 200 OK with response model as body.

Response HTTP success status code can be configured by [calling 'Produces' extension method during configuration](../samples/ShowcaseWebApi/Features/Books/CreateBook.cs) of endpoint with one of the following status codes:
- StatusCodes.Status200OK,
- StatusCodes.Status201Created,
- StatusCodes.Status202Accepted,
- StatusCodes.Status204NoContent,
- StatusCodes.Status205ResetContent

When result returned from handler method is in Failed state, default WebResultEndpoint response mapping will create a Minimal API IResult with a 4XX or 5XX HTTP Status Code depending on the FailureType of [business result](https://github.com/modabas/ModResults).

It is also possible to implement a custom response mapping behaviour for a WebResultEndpoint. To do so:
- Create an IResultToResponseMapper implementation,
- Add it to dependency injection service collection with a string key during app startup,
- Apply ResultToResponseMapper attribute to endpoint classes that will be using custom mapper. Use service registration string key as Name property of attribute.