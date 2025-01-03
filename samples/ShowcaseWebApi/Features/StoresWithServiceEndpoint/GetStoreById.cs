using FluentValidation;
using Microsoft.EntityFrameworkCore;
using ModEndpoints;
using ModEndpoints.Core;
using ModResults;
using ShowcaseWebApi.Data;
using ShowcaseWebApi.FeatureContracts.Features.StoresWithServiceEndpoint;
using ShowcaseWebApi.Features.StoresWithServiceEndpoint.Configuration;

namespace ShowcaseWebApi.Features.StoresWithServiceEndpoint;

internal class GetStoreByIdRequestValidator : AbstractValidator<GetStoreByIdRequest>
{
  public GetStoreByIdRequestValidator()
  {
    RuleFor(x => x.Id).NotEmpty();
  }
}

[RouteGroupMember(typeof(StoresWithServiceEndpointRouteGroup))]
internal class GetStoreById(ServiceDbContext db)
  : ServiceEndpoint<GetStoreByIdRequest, GetStoreByIdResponse>
{
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

