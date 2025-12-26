using ModEndpoints.Core;

namespace ModEndpoints.TestServer.Customers.Configuration;

internal class CustomersRouteGroup : RouteGroupConfigurator
{
  protected override void Configure(
    RouteGroupConfigurationBuilder builder,
    RouteGroupConfigurationContext configurationContext)
  {
    builder.MapGroup("/api/customers");
  }
}
