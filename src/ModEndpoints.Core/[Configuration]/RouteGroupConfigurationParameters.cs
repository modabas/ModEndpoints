namespace ModEndpoints.Core;

public class RouteGroupConfigurationParameters
{
  public IRouteGroupConfiguratorMarker CurrentRouteGroup { get; init; }

  public int SelfDiscriminator { get; internal set; }

  public RouteGroupConfigurationParameters? ParentRouteGroupParameters { get; init; }

  public Dictionary<string, object?> PropertyBag { get; } = new();

  public RouteGroupConfigurationParameters(
    IRouteGroupConfiguratorMarker currentRouteGroup,
    int selfDiscriminator,
    RouteGroupConfigurationParameters? parentRouteGroupParameters)
  {
    CurrentRouteGroup = currentRouteGroup;
    SelfDiscriminator = selfDiscriminator;
    ParentRouteGroupParameters = parentRouteGroupParameters;
  }
}
