namespace ModEndpoints.Core;

public abstract class RouteGroupConfigurationContext
{
  public IServiceProvider ServiceProvider { get; init; }

  public RouteGroupConfigurationParameters Parameters { get; init; }

  public RouteGroupConfigurationContext(
    IServiceProvider serviceProvider,
    RouteGroupConfigurationParameters parameters)
  {
    ServiceProvider = serviceProvider;
    Parameters = parameters;
  }
}
