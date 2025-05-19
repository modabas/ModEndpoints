using ModEndpoints.Core;

namespace ModEndpoints.TestServer.Features.Books.Configuration;

[MapToGroup<FeaturesRouteGroup>()]
internal class BooksRouteGroup : RouteGroupConfigurator
{
  protected override void Configure(
    RouteGroupRegistrationBuilder builder,
    ConfigurationContext<IRouteGroupConfiguration> configurationContext)
  {
    builder.MapGroup("/books");
  }
}
