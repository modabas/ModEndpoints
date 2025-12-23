using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace ModEndpoints.Core;

/// <summary>
/// Default implementation of <see cref="EndpointConfigurationBuilder"/>.
/// </summary>
/// <param name="endpointRouteBuilder"></param>
/// <param name="executeDelegate"></param>
internal sealed class DefaultEndpointConfigurationBuilder(
  IEndpointRouteBuilder endpointRouteBuilder,
  Delegate executeDelegate)
  : EndpointConfigurationBuilder
{
  /// <summary>
  /// Holds all route handler builders created during configuration with each call to a Map[HttpVerb] method.
  /// Each builder can be further customized after being created.
  /// </summary>
  internal List<RouteHandlerBuilder> HandlerBuilders { get; private set; } = new();

  public override RouteHandlerBuilder MapGet(string pattern)
  {
    var handlerBuilder = endpointRouteBuilder.MapGet(pattern, executeDelegate).AddConfigurationMetadata();
    HandlerBuilders.Add(handlerBuilder);
    return handlerBuilder;
  }

  public override RouteHandlerBuilder MapPost(string pattern)
  {
    var handlerBuilder = endpointRouteBuilder.MapPost(pattern, executeDelegate).AddConfigurationMetadata();
    HandlerBuilders.Add(handlerBuilder);
    return handlerBuilder;
  }

  public override RouteHandlerBuilder MapDelete(string pattern)
  {
    var handlerBuilder = endpointRouteBuilder.MapDelete(pattern, executeDelegate).AddConfigurationMetadata();
    HandlerBuilders.Add(handlerBuilder);
    return handlerBuilder;
  }

  public override RouteHandlerBuilder MapPut(string pattern)
  {
    var handlerBuilder = endpointRouteBuilder.MapPut(pattern, executeDelegate).AddConfigurationMetadata();
    HandlerBuilders.Add(handlerBuilder);
    return handlerBuilder;
  }

  public override RouteHandlerBuilder MapPatch(string pattern)
  {
    var handlerBuilder = endpointRouteBuilder.MapPatch(pattern, executeDelegate).AddConfigurationMetadata();
    HandlerBuilders.Add(handlerBuilder);
    return handlerBuilder;
  }

  public override RouteHandlerBuilder MapMethods(string pattern, IEnumerable<string> httpMethods)
  {
    var handlerBuilder = endpointRouteBuilder.MapMethods(pattern, httpMethods, executeDelegate).AddConfigurationMetadata();
    HandlerBuilders.Add(handlerBuilder);
    return handlerBuilder;
  }
}
