namespace ModEndpoints.Core;

/// <summary>
/// Default implementation of <see cref="RouteGroupConfigurationContext"/>.
/// </summary>
internal sealed class DefaultRouteGroupConfigurationContext : RouteGroupConfigurationContext
{
  public override IServiceProvider ConfigurationServices { get; }

  public override RouteGroupConfigurationParameters Parameters { get; }

  public DefaultRouteGroupConfigurationContext(
    IServiceProvider serviceProvider,
    RouteGroupConfigurationParameters parameters)
  {
    ConfigurationServices = serviceProvider;
    Parameters = parameters;
  }
}
