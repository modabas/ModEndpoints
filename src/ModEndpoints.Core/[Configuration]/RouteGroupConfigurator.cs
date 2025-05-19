using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Patterns;

namespace ModEndpoints.Core;

public abstract class RouteGroupConfigurator : IRouteGroupConfigurator
{
  private IEndpointRouteBuilder? _builder;

  private RouteGroupBuilder? _groupBuilder;

  public Dictionary<string, object?> ConfigurationPropertyBag { get; private set; } = new();

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
    _builder = builder;
    ParentRouteGroup = configurationContext.ParentRouteGroup;
    Configure(configurationContext);
    return _groupBuilder;
  }

  /// <summary>
  /// Called during application startup, while registering and configuring groups.
  /// Start configuring route group by calling the MapGroup method and chain additional configuration on top of returned <see cref="RouteGroupBuilder"/>.
  /// </summary>
  /// <param name="configurationContext"></param>
  protected abstract void Configure(
    ConfigurationContext<IRouteGroupConfiguration> configurationContext);

  public virtual Action<RouteHandlerBuilder, ConfigurationContext<IEndpointConfiguration>>? EndpointConfigurationOverrides => null;

  public virtual Action<RouteGroupBuilder, ConfigurationContext<IRouteGroupConfiguration>>? ConfigurationOverrides => null;

  /// <summary>
  /// To be used in "Configure" overload method to create a <see cref="RouteGroupBuilder"/>
  /// for defining endpoints, all prefixed with <paramref name="prefix"/>
  /// </summary>
  /// <param name="prefix"></param>
  /// <returns>A <see cref="RouteGroupBuilder"/> that can be used to further customize the group.</returns>
  /// <exception cref="InvalidOperationException"></exception>
  protected RouteGroupBuilder MapGroup(string prefix)
  {
    _groupBuilder = _builder is null
      ? throw new InvalidOperationException(string.Format(Constants.RouteBuilderIsNullForRouteGroupMessage, GetType()))
      : _builder.MapGroup(prefix);
    return _groupBuilder;
  }

  /// <summary>
  /// To be used in "Configure" overload method to create a <see cref="RouteGroupBuilder"/>
  /// for defining endpoints, all prefixed with <paramref name="prefix"/>
  /// </summary>
  /// <param name="prefix"></param>
  /// <returns>A <see cref="RouteGroupBuilder"/> that can be used to further customize the group.</returns>
  /// <exception cref="InvalidOperationException"></exception>
  protected RouteGroupBuilder MapGroup(RoutePattern prefix)
  {
    _groupBuilder = _builder is null
      ? throw new InvalidOperationException(string.Format(Constants.RouteBuilderIsNullForRouteGroupMessage, GetType()))
      : _builder.MapGroup(prefix);
    return _groupBuilder;
  }
}
