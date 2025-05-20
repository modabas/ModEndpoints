using ModEndpoints.Core;
using ModEndpoints.TestServer.Features.Stores.Configuration;
using ModResults;

namespace ModEndpoints.TestServer.Features.Stores;

public record ListStoresResponse(List<ListStoresResponseItem> Stores);
public record ListStoresResponseItem(Guid Id, string Name);

[MapToGroup<StoresRouteGroup>()]
internal class ListStores
  : BusinessResultEndpointWithEmptyRequest<ListStoresResponse>
{
  protected override void Configure(
    EndpointConfigurationBuilder builder,
    ConfigurationContext<EndpointConfigurationParameters> configurationContext)
  {
    builder.MapGet("/");
  }

  protected override async Task<Result<ListStoresResponse>> HandleAsync(
    CancellationToken ct)
  {
    await Task.CompletedTask; // Simulate async work

    return new ListStoresResponse(Stores:
      [
        new ListStoresResponseItem(Guid.NewGuid(), "Name 1"),
        new ListStoresResponseItem(Guid.NewGuid(), "Name 2")
      ]);
  }
}
