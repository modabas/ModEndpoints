namespace ModEndpoints.Core;

/// <summary>
/// Context provided during route group configuration. Contains scoped service provider and parameters related to the route group being configured.
/// </summary>
public abstract class RouteGroupConfigurationContext
{
  /// <summary>
  /// Scoped service provider that can be used during configuration step if necessary.
  /// </summary>
  public IServiceProvider ServiceProvider { get; init; }

  /// <summary>
  /// Route group configuration parameters.
  /// </summary>
  public RouteGroupConfigurationParameters Parameters { get; init; }

  public RouteGroupConfigurationContext(
    IServiceProvider serviceProvider,
    RouteGroupConfigurationParameters parameters)
  {
    ServiceProvider = serviceProvider;
    Parameters = parameters;
  }
}
