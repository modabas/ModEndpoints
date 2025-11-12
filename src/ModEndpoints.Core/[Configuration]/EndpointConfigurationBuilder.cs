using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace ModEndpoints.Core;

/// <summary>
/// Contains necessary methods to configure endpoints.
/// </summary>
public abstract class EndpointConfigurationBuilder
{
  /// <summary>
  /// To be used in "Configure" overload method to add a <see cref="RouteEndpoint"/>
  /// to the application <see cref="IEndpointRouteBuilder"/>, that matches HTTP DELETE 
  /// requests for the specified pattern.
  /// </summary>
  /// <param name="pattern"></param>
  /// <returns>A <see cref="RouteHandlerBuilder"/> that can be used to further customize the endpoint.</returns>
  public abstract RouteHandlerBuilder MapDelete(string pattern);

  /// <summary>
  /// To be used in "Configure" overload method to add a <see cref="RouteEndpoint"/>
  /// to the application <see cref="IEndpointRouteBuilder"/>, that matches HTTP GET 
  /// requests for the specified pattern.
  /// </summary>
  /// <param name="pattern"></param>
  /// <returns>A <see cref="RouteHandlerBuilder"/> that can be used to further customize the endpoint.</returns>
  public abstract RouteHandlerBuilder MapGet(string pattern);

  /// <summary>
  /// To be used in "Configure" overload method to add a <see cref="RouteEndpoint"/>
  /// to the application <see cref="IEndpointRouteBuilder"/>, that matches requests with
  /// specified HTTP methods for the specified pattern.
  /// </summary>
  /// <param name="pattern"></param>
  /// <param name="httpMethods"></param>
  /// <returns>A <see cref="RouteHandlerBuilder"/> that can be used to further customize the endpoint.</returns>
  public abstract RouteHandlerBuilder MapMethods(string pattern, IEnumerable<string> httpMethods);

  /// <summary>
  /// To be used in "Configure" overload method to add a <see cref="RouteEndpoint"/>
  /// to the application <see cref="IEndpointRouteBuilder"/>, that matches HTTP PATCH 
  /// requests for the specified pattern.
  /// </summary>
  /// <param name="pattern"></param>
  /// <returns>A <see cref="RouteHandlerBuilder"/> that can be used to further customize the endpoint.</returns>
  public abstract RouteHandlerBuilder MapPatch(string pattern);

  /// <summary>
  /// To be used in "Configure" overload method to add a <see cref="RouteEndpoint"/>
  /// to the application <see cref="IEndpointRouteBuilder"/>, that matches HTTP POST 
  /// requests for the specified pattern.
  /// </summary>
  /// <param name="pattern"></param>
  /// <returns>A <see cref="RouteHandlerBuilder"/> that can be used to further customize the endpoint.</returns>
  public abstract RouteHandlerBuilder MapPost(string pattern);

  /// <summary>
  /// To be used in "Configure" overload method to add a <see cref="RouteEndpoint"/>
  /// to the application <see cref="IEndpointRouteBuilder"/>, that matches HTTP PUT 
  /// requests for the specified pattern.
  /// </summary>
  /// <param name="pattern"></param>
  /// <returns>A <see cref="RouteHandlerBuilder"/> that can be used to further customize the endpoint.</returns>
  public abstract RouteHandlerBuilder MapPut(string pattern);
}
