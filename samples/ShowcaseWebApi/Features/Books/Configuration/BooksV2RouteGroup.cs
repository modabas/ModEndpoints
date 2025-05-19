using ModEndpoints.Core;

namespace ShowcaseWebApi.Features.Books.Configuration;

[MapToGroup<FeaturesRouteGroup>()]
internal class BooksV2RouteGroup : RouteGroupConfigurator
{
  protected override void Configure(
    RouteGroupRegistrationBuilder builder,
    ConfigurationContext<IRouteGroupConfiguration> configurationContext)
  {
    builder.MapGroup("/books")
      .MapToApiVersion(2)
      .WithTags("/BooksV2");
  }
}
