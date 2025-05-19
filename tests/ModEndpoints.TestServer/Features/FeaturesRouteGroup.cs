using ModEndpoints.Core;

namespace ModEndpoints.TestServer.Features;

internal class FeaturesRouteGroup : RouteGroupConfigurator
{
  protected override void Configure(
    ConfigurationContext<IRouteGroupConfiguration> configurationContext)
  {
    MapGroup("/api");
  }
}
