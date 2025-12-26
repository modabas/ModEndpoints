namespace ModEndpoints.Core;

/// <summary>
/// Storage for endpoint configuration parameters.
/// </summary>
public abstract class EndpointConfigurationParameters
{
  /// <summary>
  /// Indicates the endpoint currently being configured.
  /// </summary>
  public abstract IEndpointConfiguratorMarker CurrentEndpoint { get; }

  /// <summary>
  /// Configure method of an endpoint can be called multiple times to register multiple routes for the same endpoint type.
  /// Contains a unique integer value that can be used to discriminate this endpoint from others of the same type.
  /// </summary>
  public abstract int SelfDiscriminator { get; set; }

  /// <summary>
  /// Indicates the parent route group parameters, if the endpoint is being configured within a route group.
  /// </summary>
  public abstract RouteGroupConfigurationParameters? ParentRouteGroupParameters { get; }

  /// <summary>
  /// A bag of custom properties that can be used to store arbitrary data related to the endpoint configuration.
  /// Passed along to child route groups and endpoints.
  /// </summary>
  public abstract Dictionary<string, object?> PropertyBag { get; }
}
