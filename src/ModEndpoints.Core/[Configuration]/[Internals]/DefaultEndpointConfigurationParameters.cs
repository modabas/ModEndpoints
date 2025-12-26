namespace ModEndpoints.Core;

/// <summary>
/// Default implementation of <see cref="EndpointConfigurationParameters"/>.
/// </summary>
internal sealed class DefaultEndpointConfigurationParameters : EndpointConfigurationParameters
{
  public override IEndpointConfiguratorMarker CurrentEndpoint { get; }
  public override int SelfDiscriminator { get; set; }
  public override RouteGroupConfigurationParameters? ParentRouteGroupParameters { get; }
  public override Dictionary<string, object?> PropertyBag { get; } = new();

  public DefaultEndpointConfigurationParameters(
    IEndpointConfiguratorMarker currentEndpoint,
    int selfDiscriminator,
    RouteGroupConfigurationParameters? parentRouteGroupParameters)
  {
    CurrentEndpoint = currentEndpoint;
    SelfDiscriminator = selfDiscriminator;
    ParentRouteGroupParameters = parentRouteGroupParameters;
  }
}
