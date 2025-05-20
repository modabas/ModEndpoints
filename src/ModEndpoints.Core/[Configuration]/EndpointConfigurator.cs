using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace ModEndpoints.Core;
public abstract class EndpointConfigurator : IEndpointConfigurator
{
  protected abstract Delegate ExecuteDelegate { get; }

  private EndpointConfigurationBuilder? _builder;

  public Dictionary<string, object?>? ConfigurationPropertyBag { get; set; }

  public virtual Action<RouteHandlerBuilder, ConfigurationContext<EndpointConfigurationParameters>>? ConfigurationOverrides => null;

  /// <summary>
  /// Entry point for endpoint configuration. Called by DI.
  /// </summary>
  /// <param name="builder"></param>
  /// <param name="configurationContext"></param>
  /// <returns>A <see cref="RouteHandlerBuilder"/> that can be used to further customize the endpoint.</returns>
  public RouteHandlerBuilder? Configure(
    IEndpointRouteBuilder builder,
    ConfigurationContext<EndpointConfigurationParameters> configurationContext)
  {
    _builder = new(builder, ExecuteDelegate);
    Configure(_builder, configurationContext);
    return _builder.HandlerBuilder;
  }

  /// <summary>
  /// Called during application startup, while registering and configuring endpoints.
  /// Start configuring endpoint by calling one of the Map[HttpVerb] methods and chain additional configuration on top of returned <see cref="RouteHandlerBuilder"/>.
  /// </summary>
  /// <param name="builder"></param>
  /// <param name="configurationContext"></param>
  protected abstract void Configure(
    EndpointConfigurationBuilder builder,
    ConfigurationContext<EndpointConfigurationParameters> configurationContext);

}
