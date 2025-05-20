using ModEndpoints.Core;

namespace ShowcaseWebApi.Features.Stores.Configuration;

[MapToGroup<FeaturesRouteGroup>()]
internal class StoresRouteGroup : RouteGroupConfigurator
{
  protected override void Configure(
    RouteGroupConfigurationBuilder builder,
    ConfigurationContext<RouteGroupConfigurationParameters> configurationContext)
  {
    builder.MapGroup("/stores")
      .MapToApiVersion(1)
      .MapToApiVersion(2)
      .WithTags("/Stores");
  }
}
