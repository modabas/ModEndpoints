using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace ModEndpoints.Core;

public abstract class ServiceEndpointConfigurator : IEndpointConfigurator
{
  protected abstract Delegate ExecuteDelegate { get; }

  /// <summary>
  /// Entry point for endpoint configuration. Called by DI.
  /// </summary>
  /// <param name="builder"></param>
  /// <param name="configurationContext"></param>
  /// <returns>A <see cref="RouteHandlerBuilder"/> that can be used to further customize the endpoint.</returns>
  public RouteHandlerBuilder[] Configure(
    IEndpointRouteBuilder builder,
    ConfigurationContext<EndpointConfigurationParameters> configurationContext)
  {
    var handlerBuilder = ConfigureDefaults(builder, configurationContext);
    handlerBuilder = ValidateRouteHandlerBuilder(handlerBuilder);
    Configure(handlerBuilder, configurationContext);
    return [handlerBuilder];
  }

  protected abstract RouteHandlerBuilder? ConfigureDefaults(
    IEndpointRouteBuilder builder,
    ConfigurationContext<EndpointConfigurationParameters> configurationContext);

  /// <summary>
  /// Called during application startup, while registering and configuring endpoints.
  /// Runs after ConfigureDefaults method and can be overridden to further customize endpoint on top of default configuration.
  /// Use <see cref="RouteHandlerBuilder"/> parameter to chain additional configuration.
  /// </summary>
  /// <param name="builder"></param>
  /// <param name="configurationContext"></param>
  protected virtual void Configure(
    RouteHandlerBuilder builder,
    ConfigurationContext<EndpointConfigurationParameters> configurationContext)
  {
    return;
  }

  /// <summary>
  /// Called during application startup, while registering and configuring endpoints.
  /// This executes after endpoint has been configured and the global endpoint configuration has completed.
  /// Can be used to modify endpoint configuration.
  /// </summary>
  /// <param name="builder"></param>
  /// <param name="configurationContext"></param>
  public virtual void PostConfigure(
    RouteHandlerBuilder builder,
    ConfigurationContext<EndpointConfigurationParameters> configurationContext)
  {
    return;
  }

  private RouteHandlerBuilder ValidateRouteHandlerBuilder(RouteHandlerBuilder? handlerBuilder)
  {
    return handlerBuilder is null
      ? throw new InvalidOperationException(string.Format(Constants.RouteBuilderIsNullForEndpointMessage, GetType()))
      : handlerBuilder;
  }
}
