namespace ModEndpoints.Core;

public class EndpointConfigurationParameters
{
  public IEndpointConfiguratorMarker CurrentEndpoint { get; init; }

  public int SelfDiscriminator { get; internal set; }

  public RouteGroupConfigurationParameters? ParentRouteGroupParameters { get; init; }

  public Dictionary<string, object?> PropertyBag { get; } = new();

  public EndpointConfigurationParameters(
    IEndpointConfiguratorMarker currentEndpoint,
    int selfDiscriminator,
    RouteGroupConfigurationParameters? parentRouteGroupParameters)
  {
    CurrentEndpoint = currentEndpoint;
    SelfDiscriminator = selfDiscriminator;
    ParentRouteGroupParameters = parentRouteGroupParameters;
  }
}
