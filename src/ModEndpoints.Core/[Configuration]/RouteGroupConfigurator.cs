using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace ModEndpoints.Core;

public abstract class RouteGroupConfigurator : IRouteGroupConfigurator
{
  private RouteGroupConfigurationBuilder? _configurationBuilder;

  public virtual Action<RouteHandlerBuilder, ConfigurationContext<EndpointConfigurationParameters>>? EndpointConfigurationOverrides => null;

  public virtual Action<RouteGroupBuilder, ConfigurationContext<RouteGroupConfigurationParameters>>? ConfigurationOverrides => null;

  /// <summary>
  /// Entry point for route group configuration. Called by DI.
  /// </summary>
  /// <param name="builder"></param>
  /// <param name="configurationContext"></param>
  /// <returns>A <see cref="RouteGroupBuilder"/> that can be used to further customize the route group.</returns>
  public RouteGroupBuilder? Configure(
    IEndpointRouteBuilder builder,
    ConfigurationContext<RouteGroupConfigurationParameters> configurationContext)
  {
    _configurationBuilder = new(builder);
    Configure(_configurationBuilder, configurationContext);
    return _configurationBuilder.GroupBuilder;
  }

  /// <summary>
  /// Called during application startup, while registering and configuring groups.
  /// Start configuring route group by calling the MapGroup method and chain additional configuration on top of returned <see cref="RouteGroupBuilder"/>.
  /// </summary>
  /// <param name="builder"></param>
  /// <param name="configurationContext"></param>
  protected abstract void Configure(
    RouteGroupConfigurationBuilder builder,
    ConfigurationContext<RouteGroupConfigurationParameters> configurationContext);
}
