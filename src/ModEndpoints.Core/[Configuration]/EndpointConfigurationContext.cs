namespace ModEndpoints.Core;

/// <summary>
/// Context provided during endpoint configuration. Contains scoped service provider and parameters related to the endpoint being configured.
/// </summary>
public abstract class EndpointConfigurationContext
{
  /// <summary>
  /// Service provider that can be used during configuration step if necessary.
  /// Scope of this service provider is the whole configuration process and is disposed of once all endpoints and route groups have been mapped.
  /// Therefore, any scoped service resolved from this provider will be same instance for all endpoints and route groups being configured unless a sub-scope is created manually within the configuration step.
  /// </summary>
  public IServiceProvider ServiceProvider { get; init; }

  /// <summary>
  /// Endpoint configuration parameters.
  /// </summary>
  public EndpointConfigurationParameters Parameters { get; init; }

  public EndpointConfigurationContext(
    IServiceProvider serviceProvider,
    EndpointConfigurationParameters parameters)
  {
    ServiceProvider = serviceProvider;
    Parameters = parameters;
  }
}
