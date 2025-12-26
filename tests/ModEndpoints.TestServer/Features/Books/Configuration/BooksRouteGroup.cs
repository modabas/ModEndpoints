using ModEndpoints.Core;

namespace ModEndpoints.TestServer.Features.Books.Configuration;

[MapToGroup<FeaturesRouteGroup>()]
internal class BooksRouteGroup : RouteGroupConfigurator
{
  protected override void Configure(
    RouteGroupConfigurationBuilder builder,
    RouteGroupConfigurationContext configurationContext)
  {
    builder.MapGroup("/books");
  }
}
