using ModEndpoints.Core;
using ModEndpoints.TestServer.Features.Stores.Configuration;
using ModResults;

namespace ModEndpoints.TestServer.Features.Stores;

[MapToGroup<StoresRouteGroup>()]
internal class ResultEndpointWithEmptyRequest
  : BusinessResultEndpointWithEmptyRequest
{
  protected override void Configure(
    EndpointConfigurationBuilder builder,
    ConfigurationContext<IEndpointConfigurationSettings> configurationContext)
  {
    builder.MapDelete("/");
  }

  protected override async Task<Result> HandleAsync(
    CancellationToken ct)
  {
    await Task.CompletedTask; // Simulate async work
    return Result.Ok();
  }
}
