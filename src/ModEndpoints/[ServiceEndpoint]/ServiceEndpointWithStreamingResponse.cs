using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using ModEndpoints.Core;
using ModEndpoints.RemoteServices;
using ModEndpoints.RemoteServices.Core;

namespace ModEndpoints;
public abstract class ServiceEndpointWithStreamingResponse<TRequest, TResultValue>
  : BaseServiceEndpointWithStreamingResponse<TRequest, StreamingResponseItem<TResultValue>>
  where TRequest : IServiceRequestWithStreamingResponse<TResultValue>
  where TResultValue : notnull
{
  protected override ValueTask<StreamingResponseItem<TResultValue>> HandleInvalidValidationResultAsync(
    RequestValidationResult validationResult,
    HttpContext context,
    CancellationToken ct)
  {
    return new ValueTask<StreamingResponseItem<TResultValue>>(
      new StreamingResponseItem<TResultValue>(
        validationResult.ToInvalidResult<TResultValue>()));
  }

  protected sealed override RouteHandlerBuilder? ConfigureDefaults(
    IEndpointRouteBuilder builder,
    ConfigurationContext<IEndpointConfiguration> configurationContext)
  {
    var serviceProvider = configurationContext.ConfigurationServices;
    var uriResolverProvider = serviceProvider.GetRequiredService<IUriResolverProvider>();
    var uriResolver = uriResolverProvider.GetResolver(
      serviceProvider,
      configurationContext.ParentRouteGroup,
      this);
    var patternResult = uriResolver.Resolve<TRequest>();
    if (patternResult.IsOk)
    {
      return builder.MapPost(patternResult.Value, ExecuteDelegate);
    }
    return null;
  }
}

public abstract class ServiceEndpointWithStreamingResponse<TRequest>
  : BaseServiceEndpointWithStreamingResponse<TRequest, StreamingResponseItem>
  where TRequest : IServiceRequestWithStreamingResponse
{
  protected override ValueTask<StreamingResponseItem> HandleInvalidValidationResultAsync(
    RequestValidationResult validationResult,
    HttpContext context,
    CancellationToken ct)
  {
    return new ValueTask<StreamingResponseItem>(
      new StreamingResponseItem(
        validationResult.ToInvalidResult()));
  }

  protected sealed override RouteHandlerBuilder? ConfigureDefaults(
    IEndpointRouteBuilder builder,
    ConfigurationContext<IEndpointConfiguration> configurationContext)
  {
    var serviceProvider = configurationContext.ConfigurationServices;
    var uriResolverProvider = serviceProvider.GetRequiredService<IUriResolverProvider>();
    var uriResolver = uriResolverProvider.GetResolver(
      serviceProvider,
      configurationContext.ParentRouteGroup,
      this);
    var patternResult = uriResolver.Resolve<TRequest>();
    if (patternResult.IsOk)
    {
      return builder.MapPost(patternResult.Value, ExecuteDelegate);
    }
    return null;
  }
}
