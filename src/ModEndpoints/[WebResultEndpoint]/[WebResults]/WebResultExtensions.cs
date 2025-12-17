using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using ModResults.MinimalApis;

namespace ModEndpoints;

public static class WebResultExtensions
{
  extension(WebResult webResult)
  {
    internal ValueTask<IResult> ExecuteInternalAsync(HttpContext context, string? location)
    {
      if (webResult.Result.IsFailed)
      {
        return ValueTask.FromResult(webResult.Result.ToErrorResponse());
      }

      var preferredSuccessStatusCodeCache = context.RequestServices
        .GetRequiredKeyedService<IPreferredSuccessStatusCodeCache>(
        WebResultEndpointDefinitions.DefaultPreferredSuccessStatusCodeCacheNameForResult);
      var preferredSuccessCode = preferredSuccessStatusCodeCache
        .GetStatusCode(context);

      return preferredSuccessCode switch
      {
        StatusCodes.Status204NoContent => ValueTask.FromResult(webResult.Result.ToResponse()),
        StatusCodes.Status200OK => ValueTask.FromResult(webResult.Result.ToResponse(SuccessfulResponseType.Ok)),
        StatusCodes.Status201Created => ValueTask.FromResult(webResult.Result.ToCreatedOrErrorResponse(location)),
        StatusCodes.Status202Accepted => ValueTask.FromResult(webResult.Result.ToAcceptedOrErrorResponse(location)),
        StatusCodes.Status205ResetContent => ValueTask.FromResult(webResult.Result.ToResponse(SuccessfulResponseType.ResetContent)),
        _ => ValueTask.FromResult(webResult.Result.ToResponse()),
      };
    }
  }

  extension<TValue>(WebResult<TValue> webResult)
    where TValue : notnull
  {
    public WebResult ToWebResult()
    {
      return webResult.Result.ToWebResult();
    }

    internal ValueTask<IResult> ExecuteInternalAsync(HttpContext context, string? location)
    {
      if (webResult.Result.IsFailed)
      {
        return ValueTask.FromResult(webResult.Result.ToErrorResponse());
      }

      var preferredSuccessStatusCodeCache = context.RequestServices
        .GetRequiredKeyedService<IPreferredSuccessStatusCodeCache>(
        WebResultEndpointDefinitions.DefaultPreferredSuccessStatusCodeCacheNameForResultOfT);
      var preferredSuccessCode = preferredSuccessStatusCodeCache
        .GetStatusCode(context);

      return preferredSuccessCode switch
      {
        StatusCodes.Status200OK => ValueTask.FromResult(webResult.Result.ToResponse()),
        StatusCodes.Status201Created => ValueTask.FromResult(webResult.Result.ToCreatedOrErrorResponse(location)),
        StatusCodes.Status202Accepted => ValueTask.FromResult(webResult.Result.ToAcceptedOrErrorResponse(location)),
        StatusCodes.Status204NoContent => ValueTask.FromResult(webResult.Result.ToResponse(SuccessfulResponseType.NoContent)),
        StatusCodes.Status205ResetContent => ValueTask.FromResult(webResult.Result.ToResponse(SuccessfulResponseType.ResetContent)),
        _ => ValueTask.FromResult(webResult.Result.ToResponse()),
      };
    }
  }
}
