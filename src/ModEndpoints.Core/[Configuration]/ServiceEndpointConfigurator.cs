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
    _handlerBuilder = MapEndpoint(serviceProvider, builder, parentRouteGroup);
    Configure(serviceProvider, parentRouteGroup);
    return _handlerBuilder;
  }

  protected abstract RouteHandlerBuilder? MapEndpoint(
    IServiceProvider serviceProvider,
    IEndpointRouteBuilder builder,
    IRouteGroupConfigurator? parentRouteGroup);

  /// <summary>
  /// Called during application startup, while registering and configuring endpoints.
  /// </summary>
  /// <param name="serviceProvider"></param>
  /// <param name="parentRouteGroup"></param>
  protected virtual void Configure(
    IServiceProvider serviceProvider,
    IRouteGroupConfigurator? parentRouteGroup)
  {
    return;
  }

  protected RouteHandlerBuilder GetBuilder()
  {
    return _handlerBuilder is null
      ? throw new InvalidOperationException(string.Format(Constants.RouteBuilderIsNullForEndpointMessage, GetType()))
      : _handlerBuilder;
  }
}
