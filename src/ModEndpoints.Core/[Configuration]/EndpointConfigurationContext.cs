namespace ModEndpoints.Core;

/// <summary>
/// Context provided during endpoint configuration. Contains scoped service provider and parameters related to the endpoint being configured.
/// </summary>
public abstract class EndpointConfigurationContext
{
  /// <summary>
  /// Scoped service provider that can be used during configuration step if necessary.
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
