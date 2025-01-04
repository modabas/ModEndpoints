using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace ModEndpoints.Core;
public abstract class EndpointConfigurator : IEndpointConfigurator
{
  protected abstract Delegate ExecuteDelegate { get; }

  private IEndpointRouteBuilder? _builder;

  private RouteHandlerBuilder? _handlerBuilder;

  public Dictionary<string, object?>? PropertyBag { get; set; }

  public virtual Action<IServiceProvider, RouteHandlerBuilder, IRouteGroupConfigurator?, IEndpointConfigurator>? ConfigurationOverrides => null;

  /// <summary>
  /// Entry point for endpoint configuration. Called by DI.
  /// </summary>
  /// <param name="serviceProvider"></param>
  /// <param name="builder"></param>
  /// <param name="parentRouteGroup"></param>
  /// <returns>A <see cref="RouteHandlerBuilder"/> that can be used to further customize the endpoint.</returns>
  public RouteHandlerBuilder? Configure(
    IServiceProvider serviceProvider,
    IEndpointRouteBuilder builder,
    IRouteGroupConfigurator? parentRouteGroup)
  {
    _builder = builder;
    Configure(serviceProvider, parentRouteGroup);
    return _handlerBuilder;
  }

  /// <summary>
  /// Called during application startup, while registering and configuring endpoints.
  /// Start configuring endpoint by calling one of the Map[HttpVerb] methods and chain additional configuration on top of returned <see cref="RouteHandlerBuilder"/>.
  /// </summary>
  /// <param name="serviceProvider"></param>
  /// <param name="parentRouteGroup"></param>
  protected abstract void Configure(
    IServiceProvider serviceProvider,
    IRouteGroupConfigurator? parentRouteGroup);

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
}
