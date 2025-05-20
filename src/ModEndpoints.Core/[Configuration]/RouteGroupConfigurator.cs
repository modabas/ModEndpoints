using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace ModEndpoints.Core;

public abstract class RouteGroupConfigurator : IRouteGroupConfigurator
{
  private RouteGroupConfigurationBuilder? _configurationBuilder;

  public Dictionary<string, object?> ConfigurationPropertyBag { get; set; } = new();

  public virtual Action<RouteHandlerBuilder, ConfigurationContext<IEndpointConfigurationSettings>>? EndpointConfigurationOverrides => null;

  public virtual Action<RouteGroupBuilder, ConfigurationContext<IRouteGroupConfigurationSettings>>? ConfigurationOverrides => null;

  public IRouteGroupConfigurationSettings? ParentRouteGroup { get; private set; }

  /// <summary>
  /// Entry point for route group configuration. Called by DI.
  /// </summary>
  /// <param name="builder"></param>
  /// <param name="configurationContext"></param>
  /// <returns>A <see cref="RouteGroupBuilder"/> that can be used to further customize the route group.</returns>
  public RouteGroupBuilder? Configure(
    IEndpointRouteBuilder builder,
    ConfigurationContext<IRouteGroupConfigurationSettings> configurationContext)
  {
    _configurationBuilder = new(builder);
    ParentRouteGroup = configurationContext.ParentRouteGroup;
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
    ConfigurationContext<IRouteGroupConfigurationSettings> configurationContext);
}
