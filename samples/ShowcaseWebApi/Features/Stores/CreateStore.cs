using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using ModEndpoints;
using ModEndpoints.Core;
using ModResults;
using ShowcaseWebApi.Data;
using ShowcaseWebApi.Features.Stores.Configuration;
using ShowcaseWebApi.Features.Stores.Data;

namespace ShowcaseWebApi.Features.Stores;
public record CreateStoreRequest([FromBody] CreateStoreRequestBody Body);
public record CreateStoreRequestBody(string Name);
public record CreateStoreResponse(Guid Id);

internal class CreateStoreRequestValidator : AbstractValidator<CreateStoreRequest>
{
  public CreateStoreRequestValidator()
  {
    RuleFor(x => x.Body.Name).NotEmpty();
  }
}

[RouteGroupMember(typeof(StoresRouteGroup))]
internal class CreateStore(ServiceDbContext db)
  : BusinessResultEndpoint<CreateStoreRequest, CreateStoreResponse>
{
  protected override void Configure(
    IServiceProvider serviceProvider,
    IRouteGroupConfigurator? parentRouteGroup)
  {
    MapPost("/");
  }

  protected override async Task<Result<CreateStoreResponse>> HandleAsync(
    CreateStoreRequest req,
    CancellationToken ct)
  {
    var Store = new StoreEntity()
    {
      Name = req.Body.Name
    };

    db.Stores.Add(Store);
    await db.SaveChangesAsync(ct);
    return Result.Ok(new CreateStoreResponse(Store.Id));
  }
}

