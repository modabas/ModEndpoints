using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using ModResults;
using ModResults.MinimalApis;

namespace ModEndpoints;

/// <summary>
/// Used to map result of Handler method to api response in Web Result Endpoints
/// </summary>
public class DefaultResultToResponseMapper : IResultToResponseMapper
{
  public async ValueTask<IResult> ToResponseAsync(
    Result result,
    HttpContext context,
    CancellationToken ct)
  {
    if (result.IsFailed)
    {
      return result.ToErrorResponse();
    }

    var preferredSuccessStatusCodeCache = context.RequestServices
      .GetRequiredKeyedService<IPreferredSuccessStatusCodeCache>(
      WebResultEndpointDefinitions.DefaultPreferredSuccessStatusCodeCacheNameForResult);
    var preferredSuccessCode = preferredSuccessStatusCodeCache
      .GetStatusCode(context);

    switch (preferredSuccessCode)
    {
      case StatusCodes.Status204NoContent:
        return result.ToResponse();
      case StatusCodes.Status200OK:
        return result.ToResponse(SuccessfulResponseType.Ok);
      case StatusCodes.Status201Created:
        {
          var locationStore = context.RequestServices.GetService<ILocationStore>();
          string? location = null;
          if (locationStore is not null)
          {
            location = await locationStore.GetValueAsync(ct);
          }
          return result.ToCreatedOrErrorResponse(location);
        }
      case StatusCodes.Status202Accepted:
        {
          var locationStore = context.RequestServices.GetService<ILocationStore>();
          string? location = null;
          if (locationStore is not null)
          {
            location = await locationStore.GetValueAsync(ct);
          }
          return result.ToAcceptedOrErrorResponse(location);
        }
      case StatusCodes.Status205ResetContent:
        return result.ToResponse(SuccessfulResponseType.ResetContent);
      default:
        return result.ToResponse();
    }
  }

  public async ValueTask<IResult> ToResponseAsync<TValue>(
    Result<TValue> result,
    HttpContext context,
    CancellationToken ct)
    where TValue : notnull
  {
    if (result.IsFailed)
    {
      return result.ToErrorResponse();
    }

    var preferredSuccessStatusCodeCache = context.RequestServices
      .GetRequiredKeyedService<IPreferredSuccessStatusCodeCache>(
      WebResultEndpointDefinitions.DefaultPreferredSuccessStatusCodeCacheNameForResultOfT);
    var preferredSuccessCode = preferredSuccessStatusCodeCache
      .GetStatusCode(context);

    switch (preferredSuccessCode)
    {
      case StatusCodes.Status200OK:
        return result.ToResponse();
      case StatusCodes.Status201Created:
        {
          var locationStore = context.RequestServices.GetService<ILocationStore>();
          string? location = null;
          if (locationStore is not null)
          {
            location = await locationStore.GetValueAsync(ct);
          }
          return result.ToCreatedOrErrorResponse(location);
        }
      case StatusCodes.Status202Accepted:
        {
          var locationStore = context.RequestServices.GetService<ILocationStore>();
          string? location = null;
          if (locationStore is not null)
          {
            location = await locationStore.GetValueAsync(ct);
          }
          return result.ToAcceptedOrErrorResponse(location);
        }
      case StatusCodes.Status204NoContent:
        return result.ToResponse(SuccessfulResponseType.NoContent);
      case StatusCodes.Status205ResetContent:
        return result.ToResponse(SuccessfulResponseType.ResetContent);
      default:
        return result.ToResponse();
    }
  }
}
