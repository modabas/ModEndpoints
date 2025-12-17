using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using ModResults;
using ModResults.MinimalApis;

namespace ModEndpoints;

public static class WebResultExtensions
{
  extension(WebResult webResult)
  {
    public WebResult AsBase()
    {
      return webResult;
    }

    internal IResult ExecuteInternal(HttpContext context, string? location)
    {
      if (webResult.Result.IsFailed)
      {
        return webResult.Result.ToErrorResponse();
      }

      var preferredSuccessStatusCodeCache = context.RequestServices
        .GetRequiredKeyedService<IPreferredSuccessStatusCodeCache>(
        WebResultEndpointDefinitions.DefaultPreferredSuccessStatusCodeCacheNameForResult);
      var preferredSuccessCode = preferredSuccessStatusCodeCache
        .GetStatusCode(context);

      return preferredSuccessCode switch
      {
        StatusCodes.Status204NoContent => webResult.Result.ToResponse(),
        StatusCodes.Status200OK => webResult.Result.ToResponse(SuccessfulResponseType.Ok),
        StatusCodes.Status201Created => webResult.Result.ToCreatedOrErrorResponse(location),
        StatusCodes.Status202Accepted => webResult.Result.ToAcceptedOrErrorResponse(location),
        StatusCodes.Status205ResetContent => webResult.Result.ToResponse(SuccessfulResponseType.ResetContent),
        _ => webResult.Result.ToResponse(),
      };
    }
  }

  extension<TValue>(WebResult<TValue> webResult)
    where TValue : notnull
  {
    public WebResult ToWebResult()
    {
      return new DefaultWebResult(webResult.Result.ToResult());
    }

    public WebResult<TValue> AsBase()
    {
      return webResult;
    }

    internal IResult ExecuteInternal(HttpContext context, string? location)
    {
      if (webResult.Result.IsFailed)
      {
        return webResult.Result.ToErrorResponse();
      }

      var preferredSuccessStatusCodeCache = context.RequestServices
        .GetRequiredKeyedService<IPreferredSuccessStatusCodeCache>(
        WebResultEndpointDefinitions.DefaultPreferredSuccessStatusCodeCacheNameForResultOfT);
      var preferredSuccessCode = preferredSuccessStatusCodeCache
        .GetStatusCode(context);

      return preferredSuccessCode switch
      {
        StatusCodes.Status200OK => webResult.Result.ToResponse(),
        StatusCodes.Status201Created => webResult.Result.ToCreatedOrErrorResponse(location),
        StatusCodes.Status202Accepted => webResult.Result.ToAcceptedOrErrorResponse(location),
        StatusCodes.Status204NoContent => webResult.Result.ToResponse(SuccessfulResponseType.NoContent),
        StatusCodes.Status205ResetContent => webResult.Result.ToResponse(SuccessfulResponseType.ResetContent),
        _ => webResult.Result.ToResponse(),
      };
    }
  }
}
