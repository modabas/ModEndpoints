using ModEndpoints.Core;

namespace ModEndpoints.TestServer.Features.StoresWithServiceEndpoint.Configuration;

[MapToGroup<FeaturesRouteGroup>()]
internal class StoresWithServiceEndpointRouteGroup : RouteGroupConfigurator
{
  protected override void Configure(
    ConfigurationContext<IRouteGroupConfiguration> configurationContext)
  {
    MapGroup("/storesWithServiceEndpoint");
  }
}
