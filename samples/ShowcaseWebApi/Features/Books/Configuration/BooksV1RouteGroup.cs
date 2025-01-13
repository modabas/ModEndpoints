using ModEndpoints.Core;

namespace ShowcaseWebApi.Features.Books.Configuration;

[MapToGroup<FeaturesRouteGroup>()]
internal class BooksV1RouteGroup : RouteGroupConfigurator
{
  protected override void Configure(
    IServiceProvider serviceProvider,
    IRouteGroupConfigurator? parentRouteGroup)
  {
    MapGroup("/books")
      .MapToApiVersion(1)
      .WithTags("/BooksV1");
  }
}
