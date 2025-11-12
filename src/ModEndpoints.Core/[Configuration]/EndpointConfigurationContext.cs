namespace ModEndpoints.Core;

public abstract class EndpointConfigurationContext
{
  public IServiceProvider ServiceProvider { get; init; }

  public EndpointConfigurationParameters Parameters { get; init; }

  public EndpointConfigurationContext(
    IServiceProvider serviceProvider,
    EndpointConfigurationParameters parameters)
  {
    ServiceProvider = serviceProvider;
    Parameters = parameters;
  }
}
