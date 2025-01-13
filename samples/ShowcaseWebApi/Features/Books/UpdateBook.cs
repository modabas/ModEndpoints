using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ModEndpoints;
using ModEndpoints.Core;
using ModResults;
using ShowcaseWebApi.Data;
using ShowcaseWebApi.Features.Books.Configuration;

namespace ShowcaseWebApi.Features.Books;
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

[MapToGroup<BooksV1RouteGroup>()]
internal class UpdateBook(ServiceDbContext db)
  : WebResultEndpoint<UpdateBookRequest, UpdateBookResponse>
{
  protected override void Configure(
    IServiceProvider serviceProvider,
    IRouteGroupConfigurator? parentRouteGroup)
  {
    MapPut("/{Id}")
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
