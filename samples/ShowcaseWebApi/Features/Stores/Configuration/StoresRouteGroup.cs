using ModEndpoints.Core;

namespace ShowcaseWebApi.Features.Stores.Configuration;

[MapToGroup<FeaturesRouteGroup>()]
internal class StoresRouteGroup : RouteGroupConfigurator
{
  protected override void Configure(
    ConfigurationContext<IRouteGroupConfiguration> configurationContext)
  {
    MapGroup("/stores")
      .MapToApiVersion(1)
      .MapToApiVersion(2)
      .WithTags("/Stores");
  }
}
