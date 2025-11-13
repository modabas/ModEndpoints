using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Patterns;

namespace ModEndpoints.Core;

/// <summary>
/// Contains necessary methods to configure route groups.
/// </summary>
public abstract class RouteGroupConfigurationBuilder
{
  /// <summary>
  /// Creates a <see cref="RouteGroupBuilder"/>
  /// for defining endpoints, all prefixed with <paramref name="prefix"/>
  /// </summary>
  /// <param name="prefix">The pattern that prefixes all routes in this group.</param>
  /// <returns>A <see cref="RouteGroupBuilder"/> that can be used to further customize the group.</returns>
  public abstract RouteGroupBuilder MapGroup(RoutePattern prefix);

  /// <summary>
  /// Creates a <see cref="RouteGroupBuilder"/>
  /// for defining endpoints, all prefixed with <paramref name="prefix"/>
  /// </summary>
  /// <param name="prefix">The pattern that prefixes all routes in this group.</param>
  /// <returns>A <see cref="RouteGroupBuilder"/> that can be used to further customize the group.</returns>
  public abstract RouteGroupBuilder MapGroup(string prefix);
}
