using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace ModEndpoints.Core;
public abstract class EndpointConfigurator : IEndpointConfigurator
{
  protected abstract Delegate ExecuteDelegate { get; }

  private IEndpointRouteBuilder? _builder;

  private RouteHandlerBuilder? _handlerBuilder;

  public Dictionary<string, object?>? ConfigurationPropertyBag { get; set; }

  public virtual Action<RouteHandlerBuilder, ConfigurationContext<IEndpointConfiguration>>? ConfigurationOverrides => null;

  /// <summary>
  /// Entry point for endpoint configuration. Called by DI.
  /// </summary>
  /// <param name="builder"></param>
  /// <param name="configurationContext"></param>
  /// <returns>A <see cref="RouteHandlerBuilder"/> that can be used to further customize the endpoint.</returns>
  public RouteHandlerBuilder? Configure(
    IEndpointRouteBuilder builder,
    ConfigurationContext<IEndpointConfiguration> configurationContext)
  {
    _builder = builder;
    Configure(configurationContext);
    return _handlerBuilder;
  }

  /// <summary>
  /// Called during application startup, while registering and configuring endpoints.
  /// Start configuring endpoint by calling one of the Map[HttpVerb] methods and chain additional configuration on top of returned <see cref="RouteHandlerBuilder"/>.
  /// </summary>
  /// <param name="configurationContext"></param>
  protected abstract void Configure(
    ConfigurationContext<IEndpointConfiguration> configurationContext);

  /// <summary>
  /// To be used in "Configure" overload method to add a <see cref="RouteEndpoint"/>
  /// to the application <see cref="IEndpointRouteBuilder"/>, that matches HTTP GET 
  /// requests for the specified pattern.
  /// </summary>
  /// <param name="pattern"></param>
  /// <returns>A <see cref="RouteHandlerBuilder"/> that can be used to further customize the endpoint.</returns>
  /// <exception cref="InvalidOperationException"></exception>
  protected RouteHandlerBuilder MapGet(string pattern)
  {
    _handlerBuilder = _builder is null
      ? throw new InvalidOperationException(string.Format(Constants.RouteBuilderIsNullForEndpointMessage, GetType()))
      : _builder.MapGet(pattern, ExecuteDelegate);
    return _handlerBuilder;
  }

  /// <summary>
  /// To be used in "Configure" overload method to add a <see cref="RouteEndpoint"/>
  /// to the application <see cref="IEndpointRouteBuilder"/>, that matches HTTP POST 
  /// requests for the specified pattern.
  /// </summary>
  /// <param name="pattern"></param>
  /// <returns>A <see cref="RouteHandlerBuilder"/> that can be used to further customize the endpoint.</returns>
  /// <exception cref="InvalidOperationException"></exception>
  protected RouteHandlerBuilder MapPost(string pattern)
  {
    _handlerBuilder = _builder is null
      ? throw new InvalidOperationException(string.Format(Constants.RouteBuilderIsNullForEndpointMessage, GetType()))
      : _builder.MapPost(pattern, ExecuteDelegate);
    return _handlerBuilder;
  }

  /// <summary>
  /// To be used in "Configure" overload method to add a <see cref="RouteEndpoint"/>
  /// to the application <see cref="IEndpointRouteBuilder"/>, that matches HTTP DELETE 
  /// requests for the specified pattern.
  /// </summary>
  /// <param name="pattern"></param>
  /// <returns>A <see cref="RouteHandlerBuilder"/> that can be used to further customize the endpoint.</returns>
  /// <exception cref="InvalidOperationException"></exception>
  protected RouteHandlerBuilder MapDelete(string pattern)
  {
    _handlerBuilder = _builder is null
      ? throw new InvalidOperationException(string.Format(Constants.RouteBuilderIsNullForEndpointMessage, GetType()))
      : _builder.MapDelete(pattern, ExecuteDelegate);
    return _handlerBuilder;
  }

  /// <summary>
  /// To be used in "Configure" overload method to add a <see cref="RouteEndpoint"/>
  /// to the application <see cref="IEndpointRouteBuilder"/>, that matches HTTP PUT 
  /// requests for the specified pattern.
  /// </summary>
  /// <param name="pattern"></param>
  /// <returns>A <see cref="RouteHandlerBuilder"/> that can be used to further customize the endpoint.</returns>
  /// <exception cref="InvalidOperationException"></exception>
  protected RouteHandlerBuilder MapPut(string pattern)
  {
    _handlerBuilder = _builder is null
      ? throw new InvalidOperationException(string.Format(Constants.RouteBuilderIsNullForEndpointMessage, GetType()))
      : _builder.MapPut(pattern, ExecuteDelegate);
    return _handlerBuilder;
  }

  /// <summary>
  /// To be used in "Configure" overload method to add a <see cref="RouteEndpoint"/>
  /// to the application <see cref="IEndpointRouteBuilder"/>, that matches HTTP PATCH 
  /// requests for the specified pattern.
  /// </summary>
  /// <param name="pattern"></param>
  /// <returns>A <see cref="RouteHandlerBuilder"/> that can be used to further customize the endpoint.</returns>
  /// <exception cref="InvalidOperationException"></exception>
  protected RouteHandlerBuilder MapPatch(string pattern)
  {
    _handlerBuilder = _builder is null
      ? throw new InvalidOperationException(string.Format(Constants.RouteBuilderIsNullForEndpointMessage, GetType()))
      : _builder.MapPatch(pattern, ExecuteDelegate);
    return _handlerBuilder;
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
  protected RouteHandlerBuilder MapMethods(string pattern, IEnumerable<string> httpMethods)
  {
    _handlerBuilder = _builder is null
      ? throw new InvalidOperationException(string.Format(Constants.RouteBuilderIsNullForEndpointMessage, GetType()))
      : _builder.MapMethods(pattern, httpMethods, ExecuteDelegate);
    return _handlerBuilder;
  }
}
