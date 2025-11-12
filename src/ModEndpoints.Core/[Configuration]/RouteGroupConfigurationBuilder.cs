using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Patterns;

namespace ModEndpoints.Core;

/// <summary>
/// Contains necessary methods to configure route groups.
/// </summary>
public abstract class RouteGroupConfigurationBuilder
{
  /// <summary>
  /// To be used in "Configure" overload method to create a <see cref="RouteGroupBuilder"/>
  /// for defining endpoints, all prefixed with <paramref name="prefix"/>
  /// </summary>
  /// <param name="prefix"></param>
  /// <returns>A <see cref="RouteGroupBuilder"/> that can be used to further customize the group.</returns>
  public abstract RouteGroupBuilder MapGroup(RoutePattern prefix);

  /// <summary>
  /// To be used in "Configure" overload method to create a <see cref="RouteGroupBuilder"/>
  /// for defining endpoints, all prefixed with <paramref name="prefix"/>
  /// </summary>
  /// <param name="prefix"></param>
  /// <returns>A <see cref="RouteGroupBuilder"/> that can be used to further customize the group.</returns>
  public abstract RouteGroupBuilder MapGroup(string prefix);
}
