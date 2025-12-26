namespace ModEndpoints.Core;

/// <summary>
/// Default implementation of <see cref="EndpointConfigurationContext"/>.
/// </summary>
internal sealed class DefaultEndpointConfigurationContext : EndpointConfigurationContext
{
  public override IServiceProvider ConfigurationServices { get; }

  public override EndpointConfigurationParameters Parameters { get; }

  public DefaultEndpointConfigurationContext(
    IServiceProvider serviceProvider,
    EndpointConfigurationParameters parameters)
  {
    ConfigurationServices = serviceProvider;
    Parameters = parameters;
  }
}
