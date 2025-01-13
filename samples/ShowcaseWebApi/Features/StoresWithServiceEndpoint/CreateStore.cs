using FluentValidation;
using ModEndpoints;
using ModEndpoints.Core;
using ModResults;
using ShowcaseWebApi.Data;
using ShowcaseWebApi.FeatureContracts.Features.StoresWithServiceEndpoint;
using ShowcaseWebApi.Features.Stores.Data;
using ShowcaseWebApi.Features.StoresWithServiceEndpoint.Configuration;

namespace ShowcaseWebApi.Features.StoresWithServiceEndpoint;
internal class CreateStoreRequestValidator : AbstractValidator<CreateStoreRequest>
{
  public CreateStoreRequestValidator()
  {
    RuleFor(x => x.Name).NotEmpty();
  }
}

[MapToGroup<StoresWithServiceEndpointRouteGroup>()]
internal class CreateStore(ServiceDbContext db)
  : ServiceEndpoint<CreateStoreRequest, CreateStoreResponse>
{
  protected override async Task<Result<CreateStoreResponse>> HandleAsync(
    CreateStoreRequest req,
    CancellationToken ct)
  {
    var Store = new StoreEntity()
    {
      Name = req.Name
    };

    db.Stores.Add(Store);
    await db.SaveChangesAsync(ct);
    return Result.Ok(new CreateStoreResponse(Store.Id));
  }
}

