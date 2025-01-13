using ModEndpoints.Core;

namespace ShowcaseWebApi.Features.Stores.Configuration;

[MapToGroup<FeaturesRouteGroup>()]
internal class StoresRouteGroup : RouteGroupConfigurator
{
  protected override void Configure(
    IServiceProvider serviceProvider,
    IRouteGroupConfigurator? parentRouteGroup)
  {
    MapGroup("/stores")
      .MapToApiVersion(1)
      .MapToApiVersion(2)
      .WithTags("/Stores");
  }
}
