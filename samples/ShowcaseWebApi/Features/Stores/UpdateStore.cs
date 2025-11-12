using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ModEndpoints;
using ModEndpoints.Core;
using ModResults;
using ShowcaseWebApi.Data;
using ShowcaseWebApi.Features.Stores.Configuration;

namespace ShowcaseWebApi.Features.Stores;

public record UpdateStoreRequest(Guid Id, [FromBody] UpdateStoreRequestBody Body);

public record UpdateStoreRequestBody(string Name);

public record UpdateStoreResponse(Guid Id, string Name);

internal class UpdateStoreRequestValidator : AbstractValidator<UpdateStoreRequest>
{
  public UpdateStoreRequestValidator()
  {
    RuleFor(x => x.Id).NotEmpty();
    RuleFor(x => x.Body.Name).NotEmpty();
  }
}

[MapToGroup<StoresRouteGroup>()]
internal class UpdateStore(ServiceDbContext db)
  : BusinessResultEndpoint<UpdateStoreRequest, UpdateStoreResponse>
{
  protected override void Configure(
    EndpointConfigurationBuilder builder,
    EndpointConfigurationContext configurationContext)
  {
    builder.MapPut("/{Id}");
  }

  protected override async Task<Result<UpdateStoreResponse>> HandleAsync(
    UpdateStoreRequest req,
    CancellationToken ct)
  {
    var entity = await db.Stores.FirstOrDefaultAsync(s => s.Id == req.Id, ct);

    if (entity is null)
    {
      return Result<UpdateStoreResponse>.NotFound();
    }

    entity.Name = req.Body.Name;

    var updated = await db.SaveChangesAsync(ct);
    return updated > 0 ?
      Result.Ok(new UpdateStoreResponse(
        Id: req.Id,
        Name: req.Body.Name))
      : Result<UpdateStoreResponse>.NotFound();
  }
}
