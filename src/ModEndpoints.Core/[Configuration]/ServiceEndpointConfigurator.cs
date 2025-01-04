using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace ModEndpoints.Core;
public abstract class ServiceEndpointConfigurator : IEndpointConfigurator
{
  protected abstract Delegate ExecuteDelegate { get; }

  private RouteHandlerBuilder? _handlerBuilder;

  public Dictionary<string, object?>? PropertyBag { get; set; }

  public virtual Action<IServiceProvider, RouteHandlerBuilder, IRouteGroupConfigurator?, IEndpointConfigurator>? ConfigurationOverrides => null;

  /// <summary>
  /// Entry point for endpoint configuration. Called by DI.
  /// </summary>
  /// <param name="serviceProvider"></param>
  /// <param name="builder"></param>
  /// <param name="parentRouteGroup"></param>
  /// <returns>A <see cref="RouteHandlerBuilder"/> that can be used to further customize the endpoint.</returns>
  public RouteHandlerBuilder? Configure(
    IServiceProvider serviceProvider,
    IEndpointRouteBuilder builder,
    IRouteGroupConfigurator? parentRouteGroup)
  {
    _handlerBuilder = ConfigureDefaults(serviceProvider, builder, parentRouteGroup);
    Configure(serviceProvider, parentRouteGroup);
    return _handlerBuilder;
  }

  protected abstract RouteHandlerBuilder? ConfigureDefaults(
    IServiceProvider serviceProvider,
    IEndpointRouteBuilder builder,
    IRouteGroupConfigurator? parentRouteGroup);

  /// <summary>
  /// Called during application startup, while registering and configuring endpoints.
  /// Runs after ConfiureDefaults method and can be overridden to further customize endpoint on top of default configuration.
  /// Use <see cref="GetRouteHandlerBuilder"/> within, to get a route handler builder and chain additional configuration on top of it.
  /// </summary>
  /// <param name="serviceProvider"></param>
  /// <param name="parentRouteGroup"></param>
  protected virtual void Configure(
    IServiceProvider serviceProvider,
    IRouteGroupConfigurator? parentRouteGroup)
  {
    return;
  }

  /// <summary>
  /// 
  /// </summary>
  /// <returns>A <see cref="RouteHandlerBuilder"/> that can be used to further customize the endpoint.</returns>
  /// <exception cref="InvalidOperationException"></exception>
  protected RouteHandlerBuilder GetRouteHandlerBuilder()
  {
    return _handlerBuilder is null
      ? throw new InvalidOperationException(string.Format(Constants.RouteBuilderIsNullForEndpointMessage, GetType()))
      : _handlerBuilder;
  }
}
