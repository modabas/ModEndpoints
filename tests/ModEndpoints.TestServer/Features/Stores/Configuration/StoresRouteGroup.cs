using ModEndpoints.Core;

namespace ModEndpoints.TestServer.Features.Stores.Configuration;

[MapToGroup<FeaturesRouteGroup>()]
internal class StoresRouteGroup : RouteGroupConfigurator
{
  protected override void Configure(
    RouteGroupRegistrationBuilder builder,
    ConfigurationContext<IRouteGroupConfiguration> configurationContext)
  {
    builder.MapGroup("/stores");
  }
}
