using ModEndpoints.Core;

namespace ModEndpoints.TestServer.Customers.Configuration;

internal class CustomersRouteGroup : RouteGroupConfigurator
{
  protected override void Configure(
    RouteGroupConfigurationBuilder builder,
    ConfigurationContext<RouteGroupConfigurationParameters> configurationContext)
  {
    builder.MapGroup("/api/customers");
  }
}
