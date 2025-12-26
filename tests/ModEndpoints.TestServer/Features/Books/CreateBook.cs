using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using ModEndpoints.Core;
using ModEndpoints.TestServer.Features.Books.Configuration;

namespace ModEndpoints.TestServer.Features.Books;

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

[MapToGroup<BooksRouteGroup>()]
internal class CreateBook
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
    var bookId = Guid.NewGuid();

    return WebResults.WithLocationRouteOnSuccess(
      new CreateBookResponse(bookId),
      typeof(GetBookById).FullName,
      new { id = bookId });
  }
}
