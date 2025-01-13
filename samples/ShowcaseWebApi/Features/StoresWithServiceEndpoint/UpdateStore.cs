using FluentValidation;
using Microsoft.EntityFrameworkCore;
using ModEndpoints;
using ModEndpoints.Core;
using ModResults;
using ShowcaseWebApi.Data;
using ShowcaseWebApi.FeatureContracts.Features.StoresWithServiceEndpoint;
using ShowcaseWebApi.Features.StoresWithServiceEndpoint.Configuration;

namespace ShowcaseWebApi.Features.StoresWithServiceEndpoint;

internal class UpdateStoreRequestValidator : AbstractValidator<UpdateStoreRequest>
{
  public UpdateStoreRequestValidator()
  {
    RuleFor(x => x.Id).NotEmpty();
    RuleFor(x => x.Name).NotEmpty();
  }
}

[MapToGroup(typeof(StoresWithServiceEndpointRouteGroup))]
internal class UpdateStore(ServiceDbContext db)
  : ServiceEndpoint<UpdateStoreRequest, UpdateStoreResponse>
{
  protected override async Task<Result<UpdateStoreResponse>> HandleAsync(
    UpdateStoreRequest req,
    CancellationToken ct)
  {
    var entity = await db.Stores.FirstOrDefaultAsync(s => s.Id == req.Id, ct);

    if (entity is null)
    {
      return Result<UpdateStoreResponse>.NotFound();
    }

    entity.Name = req.Name;

    var updated = await db.SaveChangesAsync(ct);
    return updated > 0 ?
      Result.Ok(new UpdateStoreResponse(
        Id: req.Id,
        Name: req.Name))
      : Result<UpdateStoreResponse>.NotFound();
  }
}
