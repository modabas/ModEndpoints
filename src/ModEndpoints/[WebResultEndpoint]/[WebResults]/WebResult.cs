using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using ModResults;
using ModResults.MinimalApis;

namespace ModEndpoints;

public class WebResult
{
  public Result Result => _result;
  private readonly Result _result;

  public WebResult(Result result)
  {
    _result = result;
  }

  public static implicit operator WebResult(Result result)
  {
    return result.ToWebResult();
  }

  public virtual ValueTask<IResult> ExecuteAsync(HttpContext context, CancellationToken ct)
  {
    if (_result.IsFailed)
    {
      return ValueTask.FromResult(_result.ToErrorResponse());
    }

    var preferredSuccessStatusCodeCache = context.RequestServices
      .GetRequiredKeyedService<IPreferredSuccessStatusCodeCache>(
      WebResultEndpointDefinitions.DefaultPreferredSuccessStatusCodeCacheNameForResult);
    var preferredSuccessCode = preferredSuccessStatusCodeCache
      .GetStatusCode(context);

    return preferredSuccessCode switch
    {
      StatusCodes.Status204NoContent => ValueTask.FromResult(_result.ToResponse()),
      StatusCodes.Status200OK => ValueTask.FromResult(_result.ToResponse(SuccessfulResponseType.Ok)),
      StatusCodes.Status201Created => ValueTask.FromResult(_result.ToCreatedOrErrorResponse((string?)null)),
      StatusCodes.Status202Accepted => ValueTask.FromResult(_result.ToAcceptedOrErrorResponse((string?)null)),
      StatusCodes.Status205ResetContent => ValueTask.FromResult(_result.ToResponse(SuccessfulResponseType.ResetContent)),
      _ => ValueTask.FromResult(_result.ToResponse()),
    };
  }
}

public class WebResult<TValue>
  where TValue : notnull
{
  public Result<TValue> Result => _result;
  private readonly Result<TValue> _result;
  public WebResult(Result<TValue> result)
  {
    _result = result;
  }

  public static implicit operator WebResult<TValue>(TValue value)
  {
    return new WebResult<TValue>(value);
  }

  public static implicit operator WebResult<TValue>(Result<TValue> result)
  {
    return result.ToWebResultOfTValue();
  }

  public static implicit operator WebResult(WebResult<TValue> webResult)
  {
    return webResult.ToWebResult();
  }

  public virtual ValueTask<IResult> ExecuteAsync(HttpContext context, CancellationToken ct)
  {
    if (_result.IsFailed)
    {
      return ValueTask.FromResult(_result.ToErrorResponse());
    }

    var preferredSuccessStatusCodeCache = context.RequestServices
      .GetRequiredKeyedService<IPreferredSuccessStatusCodeCache>(
      WebResultEndpointDefinitions.DefaultPreferredSuccessStatusCodeCacheNameForResultOfT);
    var preferredSuccessCode = preferredSuccessStatusCodeCache
      .GetStatusCode(context);

    return preferredSuccessCode switch
    {
      StatusCodes.Status200OK => ValueTask.FromResult(_result.ToResponse()),
      StatusCodes.Status201Created => ValueTask.FromResult(_result.ToCreatedOrErrorResponse((string?)null)),
      StatusCodes.Status202Accepted => ValueTask.FromResult(_result.ToAcceptedOrErrorResponse((string?)null)),
      StatusCodes.Status204NoContent => ValueTask.FromResult(_result.ToResponse(SuccessfulResponseType.NoContent)),
      StatusCodes.Status205ResetContent => ValueTask.FromResult(_result.ToResponse(SuccessfulResponseType.ResetContent)),
      _ => ValueTask.FromResult(_result.ToResponse()),
    };
  }
}
