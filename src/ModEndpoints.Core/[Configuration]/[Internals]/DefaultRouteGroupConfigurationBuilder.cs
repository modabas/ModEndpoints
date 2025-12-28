using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Patterns;

namespace ModEndpoints.Core;

/// <summary>
/// Default implementation of <see cref="RouteGroupConfigurationBuilder"/>.
/// </summary>
/// <param name="endpointRouteBuilder"></param>
internal sealed class DefaultRouteGroupConfigurationBuilder(
  IEndpointRouteBuilder endpointRouteBuilder)
  : RouteGroupConfigurationBuilder
{
  /// <summary>
  /// Holds all route group builders created during configuration with each call to a MapGroup method.
  /// Each builder can be further customized after being created.
  /// </summary>
  internal List<RouteGroupBuilder> GroupBuilders { get; private set; } = new();

  public override RouteGroupBuilder MapGroup(string prefix)
  {
    var groupBuilder = endpointRouteBuilder.MapGroup(prefix).AddConfigurationMetadata();
    GroupBuilders.Add(groupBuilder);
    return groupBuilder;
  }

  public override RouteGroupBuilder MapGroup(RoutePattern prefix)
  {
    var groupBuilder = endpointRouteBuilder.MapGroup(prefix).AddConfigurationMetadata();
    GroupBuilders.Add(groupBuilder);
    return groupBuilder;
  }
}
