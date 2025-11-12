using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace ModEndpoints.Core;

public class EndpointConfigurationBuilder(
  IEndpointRouteBuilder endpointRouteBuilder,
  Delegate executeDelegate)
{
  internal List<RouteHandlerBuilder> HandlerBuilders { get; private set; } = new();

  /// <summary>
  /// To be used in "Configure" overload method to add a <see cref="RouteEndpoint"/>
  /// to the application <see cref="IEndpointRouteBuilder"/>, that matches HTTP GET 
  /// requests for the specified pattern.
  /// </summary>
  /// <param name="pattern"></param>
  /// <returns>A <see cref="RouteHandlerBuilder"/> that can be used to further customize the endpoint.</returns>
  /// <exception cref="InvalidOperationException"></exception>
  public RouteHandlerBuilder MapGet(string pattern)
  {
    var handlerBuilder = endpointRouteBuilder.MapGet(pattern, executeDelegate);
    HandlerBuilders.Add(handlerBuilder);
    return handlerBuilder;
  }

  /// <summary>
  /// To be used in "Configure" overload method to add a <see cref="RouteEndpoint"/>
  /// to the application <see cref="IEndpointRouteBuilder"/>, that matches HTTP POST 
  /// requests for the specified pattern.
  /// </summary>
  /// <param name="pattern"></param>
  /// <returns>A <see cref="RouteHandlerBuilder"/> that can be used to further customize the endpoint.</returns>
  /// <exception cref="InvalidOperationException"></exception>
  public RouteHandlerBuilder MapPost(string pattern)
  {
    var handlerBuilder = endpointRouteBuilder.MapPost(pattern, executeDelegate);
    HandlerBuilders.Add(handlerBuilder);
    return handlerBuilder;
  }

  /// <summary>
  /// To be used in "Configure" overload method to add a <see cref="RouteEndpoint"/>
  /// to the application <see cref="IEndpointRouteBuilder"/>, that matches HTTP DELETE 
  /// requests for the specified pattern.
  /// </summary>
  /// <param name="pattern"></param>
  /// <returns>A <see cref="RouteHandlerBuilder"/> that can be used to further customize the endpoint.</returns>
  /// <exception cref="InvalidOperationException"></exception>
  public RouteHandlerBuilder MapDelete(string pattern)
  {
    var handlerBuilder = endpointRouteBuilder.MapDelete(pattern, executeDelegate);
    HandlerBuilders.Add(handlerBuilder);
    return handlerBuilder;
  }

  /// <summary>
  /// To be used in "Configure" overload method to add a <see cref="RouteEndpoint"/>
  /// to the application <see cref="IEndpointRouteBuilder"/>, that matches HTTP PUT 
  /// requests for the specified pattern.
  /// </summary>
  /// <param name="pattern"></param>
  /// <returns>A <see cref="RouteHandlerBuilder"/> that can be used to further customize the endpoint.</returns>
  /// <exception cref="InvalidOperationException"></exception>
  public RouteHandlerBuilder MapPut(string pattern)
  {
    var handlerBuilder = endpointRouteBuilder.MapPut(pattern, executeDelegate);
    HandlerBuilders.Add(handlerBuilder);
    return handlerBuilder;
  }

  /// <summary>
  /// To be used in "Configure" overload method to add a <see cref="RouteEndpoint"/>
  /// to the application <see cref="IEndpointRouteBuilder"/>, that matches HTTP PATCH 
  /// requests for the specified pattern.
  /// </summary>
  /// <param name="pattern"></param>
  /// <returns>A <see cref="RouteHandlerBuilder"/> that can be used to further customize the endpoint.</returns>
  /// <exception cref="InvalidOperationException"></exception>
  public RouteHandlerBuilder MapPatch(string pattern)
  {
    var handlerBuilder = endpointRouteBuilder.MapPatch(pattern, executeDelegate);
    HandlerBuilders.Add(handlerBuilder);
    return handlerBuilder;
  }

  /// <summary>
  /// To be used in "Configure" overload method to add a <see cref="RouteEndpoint"/>
  /// to the application <see cref="IEndpointRouteBuilder"/>, that matches requests with
  /// specified HTTP methods for the specified pattern.
  /// </summary>
  /// <param name="pattern"></param>
  /// <param name="httpMethods"></param>
  /// <returns>A <see cref="RouteHandlerBuilder"/> that can be used to further customize the endpoint.</returns>
  /// <exception cref="InvalidOperationException"></exception>
  public RouteHandlerBuilder MapMethods(string pattern, IEnumerable<string> httpMethods)
  {
    var handlerBuilder = endpointRouteBuilder.MapMethods(pattern, httpMethods, executeDelegate);
    HandlerBuilders.Add(handlerBuilder);
    return handlerBuilder;
  }
}
