using ModEndpoints.Core;

namespace ShowcaseWebApi.Features.Stores.Configuration;

[MapToGroup<FeaturesRouteGroup>()]
internal class StoresRouteGroup : RouteGroupConfigurator
{
  protected override void Configure(
    RouteGroupRegistrationBuilder builder,
    ConfigurationContext<IRouteGroupConfiguration> configurationContext)
  {
    builder.MapGroup("/stores")
      .MapToApiVersion(1)
      .MapToApiVersion(2)
      .WithTags("/Stores");
  }
}
