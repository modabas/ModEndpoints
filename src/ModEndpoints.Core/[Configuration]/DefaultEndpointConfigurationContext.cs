namespace ModEndpoints.Core;

public sealed class DefaultEndpointConfigurationContext : EndpointConfigurationContext
{
  public DefaultEndpointConfigurationContext(
    IServiceProvider serviceProvider,
    EndpointConfigurationParameters parameters)
    : base(serviceProvider, parameters)
  {
  }
}
