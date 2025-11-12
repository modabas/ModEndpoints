using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using ModEndpoints.Core;
using ModEndpoints.RemoteServices.Core;
using ModResults;

namespace ModEndpoints;

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
    ConfigurationContext<EndpointConfigurationParameters> configurationContext)
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
    ConfigurationContext<EndpointConfigurationParameters> configurationContext)
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
