namespace ModEndpoints.Core;

/// <summary>
/// Root route marker class to let other groups and endpoints to define a membership to.
/// By default all groups and endpoints are members under root.
/// However if a group membership is defined on them via <see cref="MapToGroupAttribute{TGroup}"/>,
/// causing them to not me a member of root anymore,
/// this marker is here to enable them being member of root directly
/// in addition to their other memberships.
/// </summary>
public sealed class RootRouteGroup : RouteGroupConfigurator
{
  protected override void Configure(
    RouteGroupConfigurationBuilder builder,
    ConfigurationContext<IRouteGroupConfigurationSettings> configurationContext)
  {
    throw new NotImplementedException();
  }
}
