using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Patterns;

namespace ModEndpoints.Core;

public abstract class RouteGroupConfigurator : IRouteGroupConfigurator
{
  private IEndpointRouteBuilder? _builder;

  private RouteGroupBuilder? _groupBuilder;

  private readonly Dictionary<string, object?> _propertyBag = new();
  public Dictionary<string, object?> PropertyBag => _propertyBag;

  /// <summary>
  /// Entry point for route group configuration. Called by DI.
  /// </summary>
  /// <param name="serviceProvider"></param>
  /// <param name="builder"></param>
  /// <param name="parentRouteGroup"></param>
  /// <returns>A <see cref="RouteGroupBuilder"/> that can be used to further customize the route group.</returns>
  public RouteGroupBuilder? Configure(
    IServiceProvider serviceProvider,
    IEndpointRouteBuilder builder,
    IRouteGroupConfigurator? parentRouteGroup)
  {
    _builder = builder;
    Configure(serviceProvider, parentRouteGroup);
    return _groupBuilder;
  }

  /// <summary>
  /// Called during application startup, while registering and configuring groups.
  /// Start configuring route group by calling the MapGroup method and chain additional configuration on top of returned <see cref="RouteGroupBuilder"/>.
  /// </summary>
  /// <param name="serviceProvider"></param>
  /// <param name="parentRouteGroup">Null if this route group is registered at root.</param>
  protected abstract void Configure(
    IServiceProvider serviceProvider,
    IRouteGroupConfigurator? parentRouteGroup);

  public virtual Action<IServiceProvider, RouteHandlerBuilder, IRouteGroupConfigurator, IEndpointConfigurator>? EndpointConfigurationOverrides => null;

  public virtual Action<IServiceProvider, RouteGroupBuilder, IRouteGroupConfigurator?, IRouteGroupConfigurator>? ConfigurationOverrides => null;

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
