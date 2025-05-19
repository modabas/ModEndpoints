using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace ModEndpoints.Core;

public abstract class RouteGroupConfigurator : IRouteGroupConfigurator
{
  private RouteGroupRegistrationBuilder? _builder;

  public Dictionary<string, object?> ConfigurationPropertyBag { get; set; } = new();

  public IRouteGroupConfiguration? ParentRouteGroup { get; private set; }

  /// <summary>
  /// Entry point for route group configuration. Called by DI.
  /// </summary>
  /// <param name="builder"></param>
  /// <param name="configurationContext"></param>
  /// <returns>A <see cref="RouteGroupBuilder"/> that can be used to further customize the route group.</returns>
  public RouteGroupBuilder? Configure(
    IEndpointRouteBuilder builder,
    ConfigurationContext<IRouteGroupConfiguration> configurationContext)
  {
    _builder = new(builder);
    ParentRouteGroup = configurationContext.ParentRouteGroup;
    Configure(_builder, configurationContext);
    return _builder.GroupBuilder;
  }

  /// <summary>
  /// Called during application startup, while registering and configuring groups.
  /// Start configuring route group by calling the MapGroup method and chain additional configuration on top of returned <see cref="RouteGroupBuilder"/>.
  /// </summary>
  /// <param name="builder"></param>
  /// <param name="configurationContext"></param>
  protected abstract void Configure(
    RouteGroupRegistrationBuilder builder,
    ConfigurationContext<IRouteGroupConfiguration> configurationContext);

  public virtual Action<RouteHandlerBuilder, ConfigurationContext<IEndpointConfiguration>>? EndpointConfigurationOverrides => null;

  public virtual Action<RouteGroupBuilder, ConfigurationContext<IRouteGroupConfiguration>>? ConfigurationOverrides => null;
}
