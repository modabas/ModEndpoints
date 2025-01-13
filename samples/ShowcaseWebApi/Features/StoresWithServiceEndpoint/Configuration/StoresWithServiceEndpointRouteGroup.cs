using ModEndpoints.Core;

namespace ShowcaseWebApi.Features.StoresWithServiceEndpoint.Configuration;

[MapToGroup(typeof(FeaturesRouteGroup))]
internal class StoresWithServiceEndpointRouteGroup : RouteGroupConfigurator
{
  protected override void Configure(
    IServiceProvider serviceProvider,
    IRouteGroupConfigurator? parentRouteGroup)
  {
    MapGroup("/storesWithServiceEndpoint")
      .MapToApiVersion(1)
      .MapToApiVersion(2)
      .WithTags("/StoresWithServiceEndpoint");
  }
}
