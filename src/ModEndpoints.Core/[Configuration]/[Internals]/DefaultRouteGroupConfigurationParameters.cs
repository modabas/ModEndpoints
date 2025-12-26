namespace ModEndpoints.Core;

/// <summary>
/// Default implementation of <see cref="RouteGroupConfigurationParameters"/>.
/// </summary>
internal sealed class DefaultRouteGroupConfigurationParameters : RouteGroupConfigurationParameters
{
  public override IRouteGroupConfiguratorMarker CurrentRouteGroup { get; }
  public override int SelfDiscriminator { get; set; }
  public override RouteGroupConfigurationParameters? ParentRouteGroupParameters { get; }
  public override Dictionary<string, object?> PropertyBag { get; } = new();
  public DefaultRouteGroupConfigurationParameters(
    IRouteGroupConfiguratorMarker currentRouteGroup,
    int selfDiscriminator,
    RouteGroupConfigurationParameters? parentRouteGroupParameters)
  {
    CurrentRouteGroup = currentRouteGroup;
    SelfDiscriminator = selfDiscriminator;
    ParentRouteGroupParameters = parentRouteGroupParameters;
  }
}
