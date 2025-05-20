using ModEndpoints.Core;

namespace ShowcaseWebApi.Features.StoresWithServiceEndpoint.Configuration;

[MapToGroup<FeaturesRouteGroup>()]
internal class StoresWithServiceEndpointRouteGroup : RouteGroupConfigurator
{
  protected override void Configure(
    RouteGroupConfigurationBuilder builder,
    ConfigurationContext<IRouteGroupConfigurationSettings> configurationContext)
  {
    builder.MapGroup("/storesWithServiceEndpoint")
      .MapToApiVersion(1)
      .MapToApiVersion(2)
      .WithTags("/StoresWithServiceEndpoint");
  }
}
