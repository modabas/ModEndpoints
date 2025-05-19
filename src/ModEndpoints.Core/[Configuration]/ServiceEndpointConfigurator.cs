using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace ModEndpoints.Core;
public abstract class ServiceEndpointConfigurator : IEndpointConfigurator
{
  protected abstract Delegate ExecuteDelegate { get; }

  private RouteHandlerBuilder? _handlerBuilder;

  public Dictionary<string, object?>? ConfigurationPropertyBag { get; set; }

  public virtual Action<RouteHandlerBuilder, ConfigurationContext<IEndpointConfiguration>>? ConfigurationOverrides => null;

  /// <summary>
  /// Entry point for endpoint configuration. Called by DI.
  /// </summary>
  /// <param name="builder"></param>
  /// <param name="configurationContext"></param>
  /// <returns>A <see cref="RouteHandlerBuilder"/> that can be used to further customize the endpoint.</returns>
  public RouteHandlerBuilder? Configure(
    IEndpointRouteBuilder builder,
    ConfigurationContext<IEndpointConfiguration> configurationContext)
  {
    _handlerBuilder = ConfigureDefaults(builder, configurationContext);
    Configure(configurationContext);
    return _handlerBuilder;
  }

  protected abstract RouteHandlerBuilder? ConfigureDefaults(
    IEndpointRouteBuilder builder,
    ConfigurationContext<IEndpointConfiguration> configurationContext);

  /// <summary>
  /// Called during application startup, while registering and configuring endpoints.
  /// Runs after ConfigureDefaults method and can be overridden to further customize endpoint on top of default configuration.
  /// Start configuring endpoint by calling <see cref="GetRouteHandlerBuilder"/> method to get a <see cref="RouteHandlerBuilder"/>, and chain additional configuration on top of it.
  /// </summary>
  /// <param name="configurationContext"></param>
  protected virtual void Configure(
    ConfigurationContext<IEndpointConfiguration> configurationContext)
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
