using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace ModEndpoints.Core;

public abstract class ServiceEndpointConfigurator : IEndpointConfigurator
{
  protected abstract Delegate ExecuteDelegate { get; }

  /// <summary>
  /// Entry point for endpoint configuration. Called by DI.
  /// </summary>
  /// <param name="builder">Endpoint builder.</param>
  /// <param name="configurationContext">Configuration context.</param>
  /// <returns>A <see cref="RouteHandlerBuilder"/> that can be used to further customize the endpoint.</returns>
  public RouteHandlerBuilder[] Configure(
    IEndpointRouteBuilder builder,
    EndpointConfigurationContext configurationContext)
  {
    var handlerBuilder = ConfigureDefaults(builder, configurationContext);
    handlerBuilder = ValidateRouteHandlerBuilder(handlerBuilder);
    Configure(handlerBuilder, configurationContext);
    return [handlerBuilder];
  }

  /// <summary>
  /// Method to configure default service endpoint settings.
  /// </summary>
  /// <param name="builder">Endpoint builder.</param>
  /// <param name="configurationContext">Configuration context.</param>
  /// <returns></returns>
  protected abstract RouteHandlerBuilder? ConfigureDefaults(
    IEndpointRouteBuilder builder,
    EndpointConfigurationContext configurationContext);

  /// <summary>
  /// Called during application startup, while registering and configuring endpoints.
  /// Runs after ConfigureDefaults method and can be overridden to further customize endpoint on top of default configuration.
  /// Use <see cref="RouteHandlerBuilder"/> parameter to chain additional configuration.
  /// </summary>
  /// <param name="builder">Endpoint builder.</param>
  /// <param name="configurationContext">Configuration context.</param>
  protected virtual void Configure(
    RouteHandlerBuilder builder,
    EndpointConfigurationContext configurationContext)
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
    EndpointConfigurationContext configurationContext)
  {
    return;
  }

  private RouteHandlerBuilder ValidateRouteHandlerBuilder(RouteHandlerBuilder? handlerBuilder)
  {
    return handlerBuilder is null
      ? throw new InvalidOperationException(string.Format(Constants.RouteBuilderIsNullForEndpointMessage, GetType()))
      : handlerBuilder.AddConfigurationMetadata();
  }
}
