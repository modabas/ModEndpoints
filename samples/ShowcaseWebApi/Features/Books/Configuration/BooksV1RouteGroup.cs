using ModEndpoints.Core;

namespace ShowcaseWebApi.Features.Books.Configuration;

[MapToGroup<FeaturesRouteGroup>()]
internal class BooksV1RouteGroup : RouteGroupConfigurator
{
  protected override void Configure(
    RouteGroupConfigurationBuilder builder,
    ConfigurationContext<IRouteGroupConfigurationSettings> configurationContext)
  {
    builder.MapGroup("/books")
      .MapToApiVersion(1)
      .WithTags("/BooksV1");
  }
}
