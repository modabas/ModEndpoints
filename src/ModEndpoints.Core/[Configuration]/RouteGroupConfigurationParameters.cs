namespace ModEndpoints.Core;

/// <summary>
/// Storage for route group configuration parameters.
/// </summary>
public class RouteGroupConfigurationParameters
{
  /// <summary>
  /// Indicates the route group currently being configured.
  /// </summary>
  public IRouteGroupConfiguratorMarker CurrentRouteGroup { get; init; }

  /// <summary>
  /// Configure method of a route group can be called multiple times to register multiple groups for the same route group type.
  /// Contains a unique integer value that can be used to discriminate this route group from others of the same type.
  /// </summary>
  public int SelfDiscriminator { get; internal set; }

  /// <summary>
  /// Indicates the parent route group parameters, if the route group is being configured within another route group.
  /// </summary>
  public RouteGroupConfigurationParameters? ParentRouteGroupParameters { get; init; }

  /// <summary>
  /// A bag of custom properties that can be used to store arbitrary data related to the route group configuration.
  /// Passed along to child route groups and endpoints.
  /// </summary>
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
