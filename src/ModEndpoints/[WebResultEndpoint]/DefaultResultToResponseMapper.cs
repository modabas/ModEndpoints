using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Metadata;
using Microsoft.Extensions.DependencyInjection;
using ModResults;
using ModResults.MinimalApis;

namespace ModEndpoints;

/// <summary>
/// Used to map result of Handler method to api response in Web Result Endpoints
/// </summary>
public class DefaultResultToResponseMapper : IResultToResponseMapper
{
  private static readonly int[] _successStatusCodePriorityListForResult =
  [
    StatusCodes.Status204NoContent,
    StatusCodes.Status200OK,
    StatusCodes.Status201Created,
    StatusCodes.Status202Accepted,
    StatusCodes.Status205ResetContent
  ];

  private static readonly int[] _successStatusCodePriorityListForResultOfT =
  [
    StatusCodes.Status200OK,
    StatusCodes.Status201Created,
    StatusCodes.Status202Accepted,
    StatusCodes.Status204NoContent,
    StatusCodes.Status205ResetContent
  ];

  public async ValueTask<IResult> ToResponseAsync(
    Result result,
    HttpContext context,
    CancellationToken ct)
  {
    if (result.IsFailed)
    {
      return result.ToErrorResponse();
    }

    var producesList = context
      .GetEndpoint()?
      .Metadata
      .GetOrderedMetadata<IProducesResponseTypeMetadata>();

    if (producesList is null || producesList.Count == 0)
    {
      return result.ToResponse();
    }

    var producesCode = _successStatusCodePriorityListForResult
      .Join(
        producesList,
        outer => outer,
        inner => inner.StatusCode,
        (outer, inner) => outer)
      .FirstOrDefault();

    switch (producesCode)
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
    };
  }

  public async ValueTask<IResult> ToResponseAsync<TValue>(
    Result<TValue> result,
    HttpContext context,
    CancellationToken ct)
  {
    if (result.IsFailed)
    {
      return result.ToErrorResponse();
    }

    var producesList = context
      .GetEndpoint()?
      .Metadata
      .GetOrderedMetadata<IProducesResponseTypeMetadata>();

    if (producesList is null || producesList.Count == 0)
    {
      return result.ToResponse();
    }

    var producesCode = _successStatusCodePriorityListForResultOfT
      .Join(
        producesList,
        outer => outer,
        inner => inner.StatusCode,
        (outer, inner) => outer)
      .FirstOrDefault();

    switch (producesCode)
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
