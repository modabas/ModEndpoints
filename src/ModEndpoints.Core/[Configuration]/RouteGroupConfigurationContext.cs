namespace ModEndpoints.Core;

/// <summary>
/// Context provided during route group configuration. Contains scoped service provider and parameters related to the route group being configured.
/// </summary>
public abstract class RouteGroupConfigurationContext
{
  /// <summary>
  /// Service provider that can be used during configuration step if necessary.
  /// Scope of this service provider is the whole configuration process and is disposed of once all endpoints and route groups have been mapped.
  /// Therefore, any scoped service resolved from this provider will be same instance for all endpoints and route groups being configured unless a sub-scope is created manually within the configuration step.
  /// </summary>
  public abstract IServiceProvider ServiceProvider { get; }

  /// <summary>
  /// Route group configuration parameters.
  /// </summary>
  public abstract RouteGroupConfigurationParameters Parameters { get; }
}
