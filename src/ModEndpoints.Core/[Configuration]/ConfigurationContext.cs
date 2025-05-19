namespace ModEndpoints.Core;

public class ConfigurationContext<T>
{
  public IServiceProvider ConfigurationServices { get; init; }

  public IRouteGroupConfiguration? ParentRouteGroup { get; init; }

  public T CurrentComponent { get; init; }

  public ConfigurationContext(
    IServiceProvider configurationServices,
    IRouteGroupConfiguration? parentRouteGroup,
    T currentComponent)
  {
    ConfigurationServices = configurationServices;
    ParentRouteGroup = parentRouteGroup;
    CurrentComponent = currentComponent;
  }
}
