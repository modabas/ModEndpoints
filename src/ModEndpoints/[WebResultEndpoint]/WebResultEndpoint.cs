﻿using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using ModEndpoints.Core;
using ModResults;

namespace ModEndpoints;

public abstract class WebResultEndpoint<TRequest, TResponse>
  : BaseWebResultEndpoint<TRequest, Result<TResponse>>
  where TRequest : notnull
  where TResponse : notnull
{
  protected override async ValueTask<IResult> ConvertResultToResponseAsync(
    Result<TResponse> result,
    HttpContext context,
    CancellationToken ct)
  {
    var mapProvider = context.RequestServices.GetRequiredService<IResultToResponseMapProvider>();
    var mapper = await mapProvider.GetMapperAsync(this, context, ct);
    return await mapper.ToResponseAsync(result, context, ct);
  }

  protected override async ValueTask<IResult> HandleInvalidValidationResultAsync(
    RequestValidationResult validationResult,
    HttpContext context,
    CancellationToken ct)
  {
    var invalidResult = validationResult.ToInvalidResult<TResponse>();
    return await ConvertResultToResponseAsync(invalidResult, context, ct);
  }
}

public abstract class WebResultEndpoint<TRequest>
  : BaseWebResultEndpoint<TRequest, Result>
  where TRequest : notnull
{
  protected override async ValueTask<IResult> ConvertResultToResponseAsync(
    Result result,
    HttpContext context,
    CancellationToken ct)
  {
    var mapProvider = context.RequestServices.GetRequiredService<IResultToResponseMapProvider>();
    var mapper = await mapProvider.GetMapperAsync(this, context, ct);
    return await mapper.ToResponseAsync(result, context, ct);
  }

  protected override async ValueTask<IResult> HandleInvalidValidationResultAsync(
    RequestValidationResult validationResult,
    HttpContext context,
    CancellationToken ct)
  {
    var invalidResult = validationResult.ToInvalidResult();
    return await ConvertResultToResponseAsync(invalidResult, context, ct);
  }
}
