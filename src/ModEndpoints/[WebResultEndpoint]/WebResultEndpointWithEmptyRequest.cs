using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using ModEndpoints.Core;
using ModResults;

namespace ModEndpoints;

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
