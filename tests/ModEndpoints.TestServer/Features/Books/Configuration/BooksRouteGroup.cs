using ModEndpoints.Core;

namespace ModEndpoints.TestServer.Features.Books.Configuration;

[MapToGroup<FeaturesRouteGroup>()]
internal class BooksRouteGroup : RouteGroupConfigurator
{
  protected override void Configure(
    RouteGroupConfigurationBuilder builder,
    ConfigurationContext<RouteGroupConfigurationParameters> configurationContext)
  {
    builder.MapGroup("/books");
  }
}
