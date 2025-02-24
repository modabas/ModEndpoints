using ModEndpoints.Core;

namespace ShowcaseWebApi.Features.Customers.Configuration;

[MapToGroup<FeaturesRouteGroup>()]
internal class CustomersV1RouteGroup : RouteGroupConfigurator
{
  protected override void Configure(
    IServiceProvider serviceProvider,
    IRouteGroupConfigurator? parentRouteGroup)
  {
    MapGroup("/customers")
      .MapToApiVersion(1)
      .WithTags("/CustomersV1");
  }
}
