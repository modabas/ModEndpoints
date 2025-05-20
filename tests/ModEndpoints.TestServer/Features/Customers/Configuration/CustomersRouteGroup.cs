using ModEndpoints.Core;

namespace ModEndpoints.TestServer.Features.Customers.Configuration;

[MapToGroup<FeaturesRouteGroup>]
internal class CustomersRouteGroup : RouteGroupConfigurator
{
  protected override void Configure(
    RouteGroupConfigurationBuilder builder,
    ConfigurationContext<RouteGroupConfigurationParameters> configurationContext)
  {
    builder.MapGroup("/customers");
  }
}
