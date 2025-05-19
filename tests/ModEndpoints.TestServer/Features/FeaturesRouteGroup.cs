using ModEndpoints.Core;

namespace ModEndpoints.TestServer.Features;

internal class FeaturesRouteGroup : RouteGroupConfigurator
{
  protected override void Configure(
    RouteGroupRegistrationBuilder builder,
    ConfigurationContext<IRouteGroupConfiguration> configurationContext)
  {
    builder.MapGroup("/api");
  }
}
