using ModEndpoints.Core;

namespace ShowcaseWebApi.Features.Customers.Configuration;

[MapToGroup<FeaturesRouteGroup>()]
internal class CustomersV1RouteGroup : RouteGroupConfigurator
{
  protected override void Configure(
    RouteGroupRegistrationBuilder builder,
    ConfigurationContext<IRouteGroupConfiguration> configurationContext)
  {
    builder.MapGroup("/customers")
      .MapToApiVersion(1)
      .WithTags("/CustomersV1");
  }
}
