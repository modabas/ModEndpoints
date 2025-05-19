namespace ModEndpoints.Core;

public class ConfigurationContext<T>
{
  public IServiceProvider ServiceProvider { get; init; }

  public IRouteGroupConfiguration? ParentRouteGroup { get; init; }

  public T CurrentComponent { get; init; }

  public ConfigurationContext(
    IServiceProvider serviceProvider,
    IRouteGroupConfiguration? parentRouteGroup,
    T currentComponent)
  {
    ServiceProvider = serviceProvider;
    ParentRouteGroup = parentRouteGroup;
    CurrentComponent = currentComponent;
  }
}
