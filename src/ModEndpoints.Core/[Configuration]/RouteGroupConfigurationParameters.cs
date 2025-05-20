namespace ModEndpoints.Core;

public class RouteGroupConfigurationParameters
{
  public IRouteGroupConfiguratorMarker CurrentRouteGroup { get; init; }

  public RouteGroupConfigurationParameters? ParentRouteGroupParameters { get; init; }

  public Dictionary<string, object?> PropertyBag { get; } = new();

  public RouteGroupConfigurationParameters(
    IRouteGroupConfiguratorMarker currentRouteGroup,
    RouteGroupConfigurationParameters? parentRouteGroupParameters)
  {
    CurrentRouteGroup = currentRouteGroup;
    ParentRouteGroupParameters = parentRouteGroupParameters;
  }
}
