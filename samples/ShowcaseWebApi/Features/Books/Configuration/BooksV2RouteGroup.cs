using ModEndpoints.Core;

namespace ShowcaseWebApi.Features.Books.Configuration;

[MapToGroup<FeaturesRouteGroup>()]
internal class BooksV2RouteGroup : RouteGroupConfigurator
{
  protected override void Configure(
    ConfigurationContext<IRouteGroupConfiguration> configurationContext)
  {
    MapGroup("/books")
      .MapToApiVersion(2)
      .WithTags("/BooksV2");
  }
}
