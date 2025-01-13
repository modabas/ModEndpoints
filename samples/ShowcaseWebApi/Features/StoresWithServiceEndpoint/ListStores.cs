using Microsoft.EntityFrameworkCore;
using ModEndpoints;
using ModEndpoints.Core;
using ModResults;
using ShowcaseWebApi.Data;
using ShowcaseWebApi.FeatureContracts.Features.StoresWithServiceEndpoint;
using ShowcaseWebApi.Features.StoresWithServiceEndpoint.Configuration;

namespace ShowcaseWebApi.Features.StoresWithServiceEndpoint;

[MapToGroup<StoresWithServiceEndpointRouteGroup>()]
internal class ListStores(ServiceDbContext db)
  : ServiceEndpoint<ListStoresRequest, ListStoresResponse>
{
  protected override async Task<Result<ListStoresResponse>> HandleAsync(
    ListStoresRequest req,
    CancellationToken ct)
  {
    var stores = await db.Stores
      .Select(b => new ListStoresResponseItem(
        b.Id,
        b.Name))
      .ToListAsync(ct);

    return new ListStoresResponse(Stores: stores);
  }
}
