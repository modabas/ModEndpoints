using ModEndpoints.Core;

namespace ModEndpoints.TestServer.Features.StoresWithServiceEndpoint.Configuration;

[MapToGroup<FeaturesRouteGroup>()]
internal class StoresWithServiceEndpointRouteGroup : RouteGroupConfigurator
{
  protected override void Configure(
    RouteGroupRegistrationBuilder builder,
    ConfigurationContext<IRouteGroupConfiguration> configurationContext)
  {
    builder.MapGroup("/storesWithServiceEndpoint");
  }
}
