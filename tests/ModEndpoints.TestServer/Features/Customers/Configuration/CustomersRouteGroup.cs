using ModEndpoints.Core;

namespace ModEndpoints.TestServer.Features.Customers.Configuration;

[MapToGroup<FeaturesRouteGroup>]
internal class CustomersRouteGroup : RouteGroupConfigurator
{
  protected override void Configure(
    IServiceProvider serviceProvider,
    IRouteGroupConfigurator? parentRouteGroup)
  {
    MapGroup("/customers");
  }
}
