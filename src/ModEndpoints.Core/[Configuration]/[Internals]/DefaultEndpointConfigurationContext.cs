namespace ModEndpoints.Core;

/// <summary>
/// Default implementation of <see cref="EndpointConfigurationContext"/>.
/// </summary>
internal sealed class DefaultEndpointConfigurationContext : EndpointConfigurationContext
{
  public DefaultEndpointConfigurationContext(
    IServiceProvider serviceProvider,
    EndpointConfigurationParameters parameters)
    : base(serviceProvider, parameters)
  {
  }
}
