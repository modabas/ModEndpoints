using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using ModEndpoints;
using ModEndpoints.Core;
using ShowcaseWebApi.Data;
using ShowcaseWebApi.Features.Books.Configuration;
using ShowcaseWebApi.Features.Books.Data;

namespace ShowcaseWebApi.Features.Books;

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

[MapToGroup<BooksV1RouteGroup>()]
internal class CreateBook(ServiceDbContext db)
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
    var book = new BookEntity()
    {
      Title = req.Body.Title,
      Author = req.Body.Author,
      Price = req.Body.Price
    };

    db.Books.Add(book);
    await db.SaveChangesAsync(ct);

    return WebResults.WithLocationRouteOnSuccess(
      new CreateBookResponse(book.Id),
      typeof(GetBookById).FullName,
      new { id = book.Id });
  }
}
