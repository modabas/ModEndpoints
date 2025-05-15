using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;
using ModEndpoints;
using ModEndpoints.Core;
using ModEndpoints.RemoteServices;
using ShowcaseWebApi.Data;
using ShowcaseWebApi.FeatureContracts.Features.StoresWithServiceEndpoint;
using ShowcaseWebApi.Features.StoresWithServiceEndpoint.Configuration;

namespace ShowcaseWebApi.Features.StoresWithServiceEndpoint;

[MapToGroup<StoresWithServiceEndpointRouteGroup>()]
internal class ListStores(ServiceDbContext db)
  : ServiceEndpointWithStreamingResponse<ListStoresRequest, ListStoresResponse>
{
  protected override async IAsyncEnumerable<StreamingResponseItem<ListStoresResponse>> HandleAsync(
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
      yield return new StreamingResponseItem<ListStoresResponse>(store, "store");
      await Task.Delay(1000, ct);
    }
  }
}
