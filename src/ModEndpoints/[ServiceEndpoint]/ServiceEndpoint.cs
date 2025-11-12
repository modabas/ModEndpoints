using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using ModEndpoints.Core;
using ModEndpoints.RemoteServices;
using ModEndpoints.RemoteServices.Core;
using ModResults;

namespace ModEndpoints;

/// <summary>
/// Abstract base class for service endpoints that return a <see cref="Result{TResultValue}"/> business result from HandleAsync method wrapped in an HTTP 200 <see cref="IResult"/>.
/// <para>Service endpoints use POST HTTP method by default, and the endpoint pattern is resolved using an <see cref="IServiceEndpointUriResolver"/>.</para>
/// <para>This is a very specialized endpoint which is intended to abstract away all HTTP client and request setup, consumption and response handling when used together with its client implementation.</para>
/// </summary>
/// <typeparam name="TRequest">Request type.</typeparam>
/// <typeparam name="TResultValue">Type of the value contained by business result response.</typeparam>
public abstract class ServiceEndpoint<TRequest, TResultValue>
  : BaseServiceEndpoint<TRequest, Result<TResultValue>>
  where TRequest : IServiceRequest<TResultValue>
  where TResultValue : notnull
{
  protected override ValueTask<Result<TResultValue>> HandleInvalidValidationResultAsync(
    RequestValidationResult validationResult,
    HttpContext context,
    CancellationToken ct)
  {
    return new ValueTask<Result<TResultValue>>(
      validationResult.ToInvalidResult<TResultValue>());
  }

  protected sealed override RouteHandlerBuilder? ConfigureDefaults(
    IEndpointRouteBuilder builder,
    EndpointConfigurationContext configurationContext)
  {
    var serviceProvider = configurationContext.ServiceProvider;
    var uriResolverProvider = serviceProvider.GetRequiredService<IUriResolverProvider>();
    var uriResolver = uriResolverProvider.GetResolver(
      serviceProvider,
      configurationContext.Parameters);
    var patternResult = uriResolver.Resolve<TRequest>();
    if (patternResult.IsOk)
    {
      return builder.MapPost(patternResult.Value, ExecuteDelegate);
    }
    return null;
  }
}

/// <summary>
/// Abstract base class for service endpoints that return a <see cref="Result"/> business result from HandleAsync method wrapped in an HTTP 200 <see cref="IResult"/>.
/// <para>Service endpoints use POST HTTP method by default, and the endpoint pattern is resolved using an <see cref="IServiceEndpointUriResolver"/>.</para>
/// <para>This is a very specialized endpoint type which is intended to abstract away all HTTP client and request setup, consumption and response handling when used together with its client implementation.</para>
/// </summary>
/// <typeparam name="TRequest">Request type.</typeparam>
public abstract class ServiceEndpoint<TRequest>
  : BaseServiceEndpoint<TRequest, Result>
  where TRequest : IServiceRequest
{
  protected override ValueTask<Result> HandleInvalidValidationResultAsync(
    RequestValidationResult validationResult,
    HttpContext context,
    CancellationToken ct)
  {
    return new ValueTask<Result>(
      validationResult.ToInvalidResult());
  }

  protected sealed override RouteHandlerBuilder? ConfigureDefaults(
    IEndpointRouteBuilder builder,
    EndpointConfigurationContext configurationContext)
  {
    var serviceProvider = configurationContext.ServiceProvider;
    var uriResolverProvider = serviceProvider.GetRequiredService<IUriResolverProvider>();
    var uriResolver = uriResolverProvider.GetResolver(
      serviceProvider,
      configurationContext.Parameters);
    var patternResult = uriResolver.Resolve<TRequest>();
    if (patternResult.IsOk)
    {
      return builder.MapPost(patternResult.Value, ExecuteDelegate);
    }
    return null;
  }
}
