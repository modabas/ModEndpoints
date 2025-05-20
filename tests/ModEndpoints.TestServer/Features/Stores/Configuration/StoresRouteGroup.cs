using ModEndpoints.Core;

namespace ModEndpoints.TestServer.Features.Stores.Configuration;

[MapToGroup<FeaturesRouteGroup>()]
internal class StoresRouteGroup : RouteGroupConfigurator
{
  protected override void Configure(
    RouteGroupConfigurationBuilder builder,
    ConfigurationContext<RouteGroupConfigurationParameters> configurationContext)
  {
    builder.MapGroup("/stores");
  }
}
