using ModEndpoints.Core;

namespace ModEndpoints.TestServer.Features.StoresWithServiceEndpoint.Configuration;

[MapToGroup<FeaturesRouteGroup>()]
internal class StoresWithServiceEndpointRouteGroup : RouteGroupConfigurator
{
  protected override void Configure(
    RouteGroupConfigurationBuilder builder,
    RouteGroupConfigurationContext configurationContext)
  {
    builder.MapGroup("/storesWithServiceEndpoint");
  }
}
