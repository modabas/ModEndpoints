using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Patterns;

namespace ModEndpoints.Core;

public class RouteGroupConfigurationBuilder(IEndpointRouteBuilder endpointRouteBuilder)
{
  internal RouteGroupBuilder? GroupBuilder { get; private set; }

  /// <summary>
  /// To be used in "Configure" overload method to create a <see cref="RouteGroupBuilder"/>
  /// for defining endpoints, all prefixed with <paramref name="prefix"/>
  /// </summary>
  /// <param name="prefix"></param>
  /// <returns>A <see cref="RouteGroupBuilder"/> that can be used to further customize the group.</returns>
  /// <exception cref="InvalidOperationException"></exception>
  public RouteGroupBuilder MapGroup(string prefix)
  {
    GroupBuilder = endpointRouteBuilder is null
      ? throw new InvalidOperationException(string.Format(Constants.RouteBuilderIsNullForRouteGroupMessage, GetType()))
      : endpointRouteBuilder.MapGroup(prefix);
    return GroupBuilder;
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
    GroupBuilder = endpointRouteBuilder is null
      ? throw new InvalidOperationException(string.Format(Constants.RouteBuilderIsNullForRouteGroupMessage, GetType()))
      : endpointRouteBuilder.MapGroup(prefix);
    return GroupBuilder;
  }
}
