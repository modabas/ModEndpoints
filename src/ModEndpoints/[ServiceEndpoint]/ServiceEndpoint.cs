﻿using FluentValidation.Results;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using ModEndpoints.Core;
using ModEndpoints.RemoteServices.Core;
using ModResults;
using ModResults.FluentValidation;

namespace ModEndpoints;
public abstract class ServiceEndpoint<TRequest, TResultValue>
  : BaseServiceEndpoint<TRequest, Result<TResultValue>>
  where TRequest : IServiceRequest<TResultValue>
  where TResultValue : notnull
{
  protected override ValueTask<Result<TResultValue>> HandleInvalidValidationResultAsync(
    ValidationResult validationResult,
    HttpContext context,
    CancellationToken ct)
  {
    return new ValueTask<Result<TResultValue>>(
      validationResult.ToInvalidResult<TResultValue>());
  }

  protected sealed override RouteHandlerBuilder? ConfigureDefaults(
    IServiceProvider serviceProvider,
    IEndpointRouteBuilder builder,
    IRouteGroupConfigurator? parentRouteGroup)
  {
    var uriResolverProvider = serviceProvider.GetRequiredService<IUriResolverProvider>();
    var uriResolver = uriResolverProvider.GetResolver(
      serviceProvider,
      parentRouteGroup,
      this);
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
    ValidationResult validationResult,
    HttpContext context,
    CancellationToken ct)
  {
    return new ValueTask<Result>(
      validationResult.ToInvalidResult());
  }

  protected sealed override RouteHandlerBuilder? ConfigureDefaults(
    IServiceProvider serviceProvider,
    IEndpointRouteBuilder builder,
    IRouteGroupConfigurator? parentRouteGroup)
  {
    var uriResolverProvider = serviceProvider.GetRequiredService<IUriResolverProvider>();
    var uriResolver = uriResolverProvider.GetResolver(
      serviceProvider,
      parentRouteGroup,
      this);
    var patternResult = uriResolver.Resolve<TRequest>();
    if (patternResult.IsOk)
    {
      return builder.MapPost(patternResult.Value, ExecuteDelegate);
    }
    return null;
  }
}
