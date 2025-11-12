using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using ModEndpoints.Core;
using ModResults;

namespace ModEndpoints;

/// <summary>
/// Abstract base class for endpoints that convert a <see cref="Result{TResponse}"/> business result returned from HandleAsync method to an <see cref="IResult"/> HTTP response, depending on the business result type, state and failure type (if any).
/// </summary>
/// <typeparam name="TResponse">Type of the value contained by business result response.</typeparam>
public abstract class WebResultEndpointWithEmptyRequest<TResponse>
  : BaseWebResultEndpoint<Result<TResponse>>
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
}

/// <summary>
/// Abstract base class for endpoints that convert a <see cref="Result"/> business result returned from HandleAsync method to an <see cref="IResult"/> HTTP response, depending on the business result type, state and failure type (if any).
/// </summary>
public abstract class WebResultEndpointWithEmptyRequest
  : BaseWebResultEndpoint<Result>
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
}
