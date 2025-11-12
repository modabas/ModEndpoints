namespace ModEndpoints.Core;

public sealed class DefaultRouteGroupConfigurationContext : RouteGroupConfigurationContext
{
  public DefaultRouteGroupConfigurationContext(
    IServiceProvider serviceProvider,
    RouteGroupConfigurationParameters parameters)
    : base(serviceProvider, parameters)
  {
  }
}
