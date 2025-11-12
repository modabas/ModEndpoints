namespace ModEndpoints.Core;

/// <summary>
/// Default implementation of <see cref="RouteGroupConfigurationContext"/>.
/// </summary>
internal sealed class DefaultRouteGroupConfigurationContext : RouteGroupConfigurationContext
{
  public DefaultRouteGroupConfigurationContext(
    IServiceProvider serviceProvider,
    RouteGroupConfigurationParameters parameters)
    : base(serviceProvider, parameters)
  {
  }
}
