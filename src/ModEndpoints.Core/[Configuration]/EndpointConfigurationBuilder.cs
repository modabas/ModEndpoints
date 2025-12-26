using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace ModEndpoints.Core;

/// <summary>
/// Contains necessary methods to configure endpoints.
/// </summary>
public abstract class EndpointConfigurationBuilder
{
  /// <summary>
  /// Adds a <see cref="RouteEndpoint"/> that matches HTTP DELETE 
  /// requests for the specified pattern.
  /// </summary>
  /// <param name="pattern">The route pattern.</param>
  /// <returns>A <see cref="RouteHandlerBuilder"/> that can be used to further customize the endpoint.</returns>
  public abstract RouteHandlerBuilder MapDelete(string pattern);

  /// <summary>
  /// Adds a <see cref="RouteEndpoint"/> that matches HTTP GET 
  /// requests for the specified pattern.
  /// </summary>
  /// <param name="pattern">The route pattern.</param>
  /// <returns>A <see cref="RouteHandlerBuilder"/> that can be used to further customize the endpoint.</returns>
  public abstract RouteHandlerBuilder MapGet(string pattern);

  /// <summary>
  /// Adds a <see cref="RouteEndpoint"/> that matches HTTP requests for the 
  /// specified HTTP methods and pattern.
  /// </summary>
  /// <param name="pattern">The route pattern.</param>
  /// <param name="httpMethods">HTTP methods that the endpoint will match.</param>
  /// <returns>A <see cref="RouteHandlerBuilder"/> that can be used to further customize the endpoint.</returns>
  public abstract RouteHandlerBuilder MapMethods(string pattern, IEnumerable<string> httpMethods);

  /// <summary>
  /// Adds a <see cref="RouteEndpoint"/> that matches HTTP PATCH 
  /// requests for the specified pattern.
  /// </summary>
  /// <param name="pattern">The route pattern.</param>
  /// <returns>A <see cref="RouteHandlerBuilder"/> that can be used to further customize the endpoint.</returns>
  public abstract RouteHandlerBuilder MapPatch(string pattern);

  /// <summary>
  /// Adds a <see cref="RouteEndpoint"/> that matches HTTP POST 
  /// requests for the specified pattern.
  /// </summary>
  /// <param name="pattern">The route pattern.</param>
  /// <returns>A <see cref="RouteHandlerBuilder"/> that can be used to further customize the endpoint.</returns>
  public abstract RouteHandlerBuilder MapPost(string pattern);

  /// <summary>
  /// Adds a <see cref="RouteEndpoint"/> that matches HTTP PUT 
  /// requests for the specified pattern.
  /// </summary>
  /// <param name="pattern">The route pattern.</param>
  /// <returns>A <see cref="RouteHandlerBuilder"/> that can be used to further customize the endpoint.</returns>
  public abstract RouteHandlerBuilder MapPut(string pattern);
}
