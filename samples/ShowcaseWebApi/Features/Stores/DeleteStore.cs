using FluentValidation;
using Microsoft.EntityFrameworkCore;
using ModEndpoints;
using ModEndpoints.Core;
using ModResults;
using ShowcaseWebApi.Data;
using ShowcaseWebApi.Features.Stores.Configuration;

namespace ShowcaseWebApi.Features.Stores;
public record DeleteStoreRequest(Guid Id);

internal class DeleteStoreRequestValidator : AbstractValidator<DeleteStoreRequest>
{
  public DeleteStoreRequestValidator()
  {
    RuleFor(x => x.Id).NotEmpty();
  }
}

[RouteGroupMember(typeof(StoresRouteGroup))]
internal class DeleteStore(ServiceDbContext db)
  : ServiceResultEndpoint<DeleteStoreRequest>
{
  protected override void Configure(
    IServiceProvider serviceProvider,
    IRouteGroupConfigurator? parentRouteGroup)
  {
    MapDelete("/{Id}");
  }

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
