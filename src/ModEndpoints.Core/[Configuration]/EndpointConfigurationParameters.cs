namespace ModEndpoints.Core;

public class EndpointConfigurationParameters
{
  public IEndpointConfiguratorMarker CurrentEndpoint { get; init; }

  public RouteGroupConfigurationParameters? ParentRouteGroupParameters { get; init; }

  public Dictionary<string, object?> PropertyBag { get; } = new();

  public EndpointConfigurationParameters(
    IEndpointConfiguratorMarker currentEndpoint,
    RouteGroupConfigurationParameters? parentRouteGroupParameters)
  {
    CurrentEndpoint = currentEndpoint;
    ParentRouteGroupParameters = parentRouteGroupParameters;
  }
}
