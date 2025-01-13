using FluentValidation;
using Microsoft.EntityFrameworkCore;
using ModEndpoints;
using ModEndpoints.Core;
using ModResults;
using ShowcaseWebApi.Data;
using ShowcaseWebApi.Features.Stores.Configuration;

namespace ShowcaseWebApi.Features.Stores;
public record GetStoreByIdRequest(Guid Id);

public record GetStoreByIdResponse(Guid Id, string Name);

internal class GetStoreByIdRequestValidator : AbstractValidator<GetStoreByIdRequest>
{
  public GetStoreByIdRequestValidator()
  {
    RuleFor(x => x.Id).NotEmpty();
  }
}

[MapToGroup(typeof(StoresRouteGroup))]
internal class GetStoreById(ServiceDbContext db)
  : BusinessResultEndpoint<GetStoreByIdRequest, GetStoreByIdResponse>
{
  protected override void Configure(
    IServiceProvider serviceProvider,
    IRouteGroupConfigurator? parentRouteGroup)
  {
    MapGet("/{Id}");
  }

  protected override async Task<Result<GetStoreByIdResponse>> HandleAsync(
    GetStoreByIdRequest req,
    CancellationToken ct)
  {
    var entity = await db.Stores.FirstOrDefaultAsync(s => s.Id == req.Id, ct);

    var result = entity is null ?
      Result<GetStoreByIdResponse>.NotFound() :
      Result.Ok(new GetStoreByIdResponse(
        Id: entity.Id,
        Name: entity.Name));

    return result;
  }
}

