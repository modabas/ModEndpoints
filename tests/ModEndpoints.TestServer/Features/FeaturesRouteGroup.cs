using ModEndpoints.Core;

namespace ModEndpoints.TestServer.Features;

internal class FeaturesRouteGroup : RouteGroupConfigurator
{
  protected override void Configure(
    RouteGroupConfigurationBuilder builder,
    ConfigurationContext<IRouteGroupConfigurationSettings> configurationContext)
  {
    builder.MapGroup("/api");
  }
}
