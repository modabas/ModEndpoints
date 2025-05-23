using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Patterns;

namespace ModEndpoints.Core;

public class RouteGroupConfigurationBuilder(IEndpointRouteBuilder endpointRouteBuilder)
{
  internal List<RouteGroupBuilder> GroupBuilders { get; private set; } = new();

  /// <summary>
  /// To be used in "Configure" overload method to create a <see cref="RouteGroupBuilder"/>
  /// for defining endpoints, all prefixed with <paramref name="prefix"/>
  /// </summary>
  /// <param name="prefix"></param>
  /// <returns>A <see cref="RouteGroupBuilder"/> that can be used to further customize the group.</returns>
  /// <exception cref="InvalidOperationException"></exception>
  public RouteGroupBuilder MapGroup(string prefix)
  {
    var groupBuilder = endpointRouteBuilder.MapGroup(prefix);
    GroupBuilders.Add(groupBuilder);
    return groupBuilder;
  }

  /// <summary>
  /// To be used in "Configure" overload method to create a <see cref="RouteGroupBuilder"/>
  /// for defining endpoints, all prefixed with <paramref name="prefix"/>
  /// </summary>
  /// <param name="prefix"></param>
  /// <returns>A <see cref="RouteGroupBuilder"/> that can be used to further customize the group.</returns>
  /// <exception cref="InvalidOperationException"></exception>
  public RouteGroupBuilder MapGroup(RoutePattern prefix)
  {
    var groupBuilder = endpointRouteBuilder.MapGroup(prefix);
    GroupBuilders.Add(groupBuilder);
    return groupBuilder;
  }
}
