namespace ModEndpoints.Core;

/// <summary>
/// Storage for endpoint configuration parameters.
/// </summary>
public class EndpointConfigurationParameters
{
  /// <summary>
  /// Indicates the endpoint currently being configured.
  /// </summary>
  public IEndpointConfiguratorMarker CurrentEndpoint { get; init; }

  /// <summary>
  /// Configure method of an endpoint can be called multiple times to register multiple routes for the same endpoint type.
  /// Contains a unique integer value that can be used to discriminate this endpoint from others of the same type.
  /// </summary>
  public int SelfDiscriminator { get; internal set; }

  /// <summary>
  /// Indicates the parent route group parameters, if the endpoint is being configured within a route group.
  /// </summary>
  public RouteGroupConfigurationParameters? ParentRouteGroupParameters { get; init; }

  /// <summary>
  /// A bag of custom properties that can be used to store arbitrary data related to the endpoint configuration.
  /// Passed along to child route groups and endpoints.
  /// </summary>
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
