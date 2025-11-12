using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace ModEndpoints.Core;

public abstract class RouteGroupConfigurator : IRouteGroupConfigurator
{
  /// <summary>
  /// Entry point for route group configuration. Called by DI.
  /// </summary>
  /// <param name="builder"></param>
  /// <param name="configurationContext"></param>
  /// <returns>A <see cref="RouteGroupBuilder"/> that can be used to further customize the route group.</returns>
  public RouteGroupBuilder[] Configure(
    IEndpointRouteBuilder builder,
    RouteGroupConfigurationContext configurationContext)
  {
    RouteGroupConfigurationBuilder configurationBuilder = new(builder);
    Configure(configurationBuilder, configurationContext);
    return configurationBuilder.GroupBuilders.ToArray();
  }

  /// <summary>
  /// Called during application startup, while registering and configuring groups.
  /// Start configuring route group by calling the MapGroup method and chain additional configuration on top of returned <see cref="RouteGroupBuilder"/>.
  /// </summary>
  /// <param name="builder"></param>
  /// <param name="configurationContext"></param>
  protected abstract void Configure(
    RouteGroupConfigurationBuilder builder,
    RouteGroupConfigurationContext configurationContext);

  /// <summary>
  /// Called during application startup, while registering and configuring groups.
  /// Runs after all child groups and endpoints have been fully configured.
  /// Can be used to modify group configuration.
  /// </summary>
  /// <param name="builder"></param>
  /// <param name="configurationContext"></param>
  public virtual void PostConfigure(
    RouteGroupBuilder builder,
    RouteGroupConfigurationContext configurationContext)
  {
    return;
  }

  /// <summary>
  /// Called during application startup, while registering and configuring groups.
  /// This executes for each endpoint directly under this route group, after endpoint has been configured, the global endpoint configuration has completed, and endpoint's own configuration overrides have run.
  /// Can be used to modify child endpoint configuration.
  /// </summary>
  /// <param name="builder"></param>
  /// <param name="configurationContext"></param>
  public virtual void EndpointPostConfigure(
    RouteHandlerBuilder builder,
    EndpointConfigurationContext configurationContext)
  {
    return;
  }
}
