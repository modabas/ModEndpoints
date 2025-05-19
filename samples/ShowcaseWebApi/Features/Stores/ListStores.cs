using Microsoft.EntityFrameworkCore;
using ModEndpoints;
using ModEndpoints.Core;
using ModResults;
using ShowcaseWebApi.Data;
using ShowcaseWebApi.Features.Stores.Configuration;

namespace ShowcaseWebApi.Features.Stores;

public record ListStoresResponse(List<ListStoresResponseItem> Stores);
public record ListStoresResponseItem(Guid Id, string Name);

[MapToGroup<StoresRouteGroup>()]
internal class ListStores(ServiceDbContext db)
  : BusinessResultEndpointWithEmptyRequest<ListStoresResponse>
{
  protected override void Configure(
    EndpointRegistrationBuilder builder,
    ConfigurationContext<IEndpointConfiguration> configurationContext)
  {
    builder.MapGet("/");
  }

  protected override async Task<Result<ListStoresResponse>> HandleAsync(
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
