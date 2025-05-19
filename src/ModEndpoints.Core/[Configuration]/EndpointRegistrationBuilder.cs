using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace ModEndpoints.Core;
public class EndpointRegistrationBuilder(IEndpointRouteBuilder endpointRouteBuilder, Delegate executeDelegate)
{
  internal RouteHandlerBuilder? HandlerBuilder { get; private set; }

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
    HandlerBuilder = endpointRouteBuilder is null
      ? throw new InvalidOperationException(string.Format(Constants.RouteBuilderIsNullForEndpointMessage, GetType()))
      : endpointRouteBuilder.MapGet(pattern, executeDelegate);
    return HandlerBuilder;
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
    HandlerBuilder = endpointRouteBuilder is null
      ? throw new InvalidOperationException(string.Format(Constants.RouteBuilderIsNullForEndpointMessage, GetType()))
      : endpointRouteBuilder.MapPost(pattern, executeDelegate);
    return HandlerBuilder;
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
    HandlerBuilder = endpointRouteBuilder is null
      ? throw new InvalidOperationException(string.Format(Constants.RouteBuilderIsNullForEndpointMessage, GetType()))
      : endpointRouteBuilder.MapDelete(pattern, executeDelegate);
    return HandlerBuilder;
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
    HandlerBuilder = endpointRouteBuilder is null
      ? throw new InvalidOperationException(string.Format(Constants.RouteBuilderIsNullForEndpointMessage, GetType()))
      : endpointRouteBuilder.MapPut(pattern, executeDelegate);
    return HandlerBuilder;
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
    HandlerBuilder = endpointRouteBuilder is null
      ? throw new InvalidOperationException(string.Format(Constants.RouteBuilderIsNullForEndpointMessage, GetType()))
      : endpointRouteBuilder.MapPatch(pattern, executeDelegate);
    return HandlerBuilder;
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
    HandlerBuilder = endpointRouteBuilder is null
      ? throw new InvalidOperationException(string.Format(Constants.RouteBuilderIsNullForEndpointMessage, GetType()))
      : endpointRouteBuilder.MapMethods(pattern, httpMethods, executeDelegate);
    return HandlerBuilder;
  }
}
