using ModEndpoints.Core;

namespace ModEndpoints.TestServer.Features.Customers.Configuration;

[MapToGroup<FeaturesRouteGroup>]
internal class CustomersRouteGroup : RouteGroupConfigurator
{
  protected override void Configure(
    RouteGroupRegistrationBuilder builder,
    ConfigurationContext<IRouteGroupConfiguration> configurationContext)
  {
    builder.MapGroup("/customers");
  }
}
