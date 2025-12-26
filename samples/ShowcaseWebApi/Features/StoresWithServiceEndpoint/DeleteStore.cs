using FluentValidation;
using Microsoft.EntityFrameworkCore;
using ModEndpoints;
using ModEndpoints.Core;
using ModResults;
using ShowcaseWebApi.Data;
using ShowcaseWebApi.FeatureContracts.Features.StoresWithServiceEndpoint;
using ShowcaseWebApi.Features.StoresWithServiceEndpoint.Configuration;

namespace ShowcaseWebApi.Features.StoresWithServiceEndpoint;

internal class DeleteStoreRequestValidator : AbstractValidator<DeleteStoreRequest>
{
  public DeleteStoreRequestValidator()
  {
    RuleFor(x => x.Id).NotEmpty();
  }
}

[MapToGroup<StoresWithServiceEndpointRouteGroup>()]
internal class DeleteStore(ServiceDbContext db)
  : ServiceEndpoint<DeleteStoreRequest>
{
  protected override async Task<Result> HandleAsync(
    DeleteStoreRequest req,
    CancellationToken ct)
  {
    var entity = await db.Stores.FirstOrDefaultAsync(s => s.Id == req.Id, ct);

    if (entity is null)
    {
      return Result.NotFound();
    }

    db.Stores.Remove(entity);
    var deleted = await db.SaveChangesAsync(ct);
    return deleted > 0 ? Result.Ok() : Result.NotFound();
  }
}
