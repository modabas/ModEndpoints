using FluentValidation;
using Microsoft.EntityFrameworkCore;
using ModEndpoints;
using ModEndpoints.Core;
using ModResults;
using ShowcaseWebApi.Data;
using ShowcaseWebApi.Features.Books.Configuration;

namespace ShowcaseWebApi.Features.Books;
public record DeleteBookRequest(Guid Id);

internal class DeleteBookRequestValidator : AbstractValidator<DeleteBookRequest>
{
  public DeleteBookRequestValidator()
  {
    RuleFor(x => x.Id).NotEmpty();
  }
}

[MapToGroup<BooksV1RouteGroup>()]
internal class DeleteBook(ServiceDbContext db)
  : WebResultEndpoint<DeleteBookRequest>
{
  protected override void Configure(
    IServiceProvider serviceProvider,
    IRouteGroupConfigurator? parentRouteGroup)
  {
    MapDelete("/{Id}");
  }

  protected override async Task<Result> HandleAsync(
    DeleteBookRequest req,
    CancellationToken ct)
  {
    var entity = await db.Books.FirstOrDefaultAsync(b => b.Id == req.Id, ct);

    if (entity is null)
    {
      return Result.NotFound();
    }

    db.Books.Remove(entity);
    var deleted = await db.SaveChangesAsync(ct);
    return deleted > 0 ? Result.Ok() : Result.NotFound();
  }
}
