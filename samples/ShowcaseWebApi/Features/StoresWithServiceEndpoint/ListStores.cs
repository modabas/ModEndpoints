using System.Runtime.CompilerServices;
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
  : ServiceEndpointWithStreamingResponse<ListStoresRequest, ListStoresResponse>
{
  protected override async IAsyncEnumerable<Result<ListStoresResponse>> HandleAsync(
    ListStoresRequest req,
    [EnumeratorCancellation] CancellationToken ct)
  {
    var stores = db.Stores
      .Select(b => new ListStoresResponse(
        b.Id,
        b.Name))
      .AsAsyncEnumerable();

    await foreach (var store in stores.WithCancellation(ct))
    {
      ct.ThrowIfCancellationRequested();
      yield return store;
    }
  }
}
