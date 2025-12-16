using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using ModResults;
using ModResults.MinimalApis;

namespace ModEndpoints;

public class WebResultAtUriOnOk : WebResult
{
  private readonly string? _location;
  public string? Location => _location;

  public WebResultAtUriOnOk(Result result, string? uri) : base(result)
  {
    _location = uri;
  }

  public WebResultAtUriOnOk(Result result, Uri? uri) : base(result)
  {
    if (uri != null)
    {
      if (uri.IsAbsoluteUri)
      {
        _location = uri.AbsoluteUri;
      }
      else
      {
        _location = uri.GetComponents(UriComponents.SerializationInfoString, UriFormat.UriEscaped);
      }
    }
  }

  public override ValueTask<IResult> ExecuteAsync(HttpContext context, CancellationToken ct)
  {
    if (Result.IsFailed)
    {
      return ValueTask.FromResult(Result.ToErrorResponse());
    }

    var preferredSuccessStatusCodeCache = context.RequestServices
      .GetRequiredKeyedService<IPreferredSuccessStatusCodeCache>(
      WebResultEndpointDefinitions.DefaultPreferredSuccessStatusCodeCacheNameForResult);
    var preferredSuccessCode = preferredSuccessStatusCodeCache
      .GetStatusCode(context);

    return preferredSuccessCode switch
    {
      StatusCodes.Status204NoContent => ValueTask.FromResult(Result.ToResponse()),
      StatusCodes.Status200OK => ValueTask.FromResult(Result.ToResponse(SuccessfulResponseType.Ok)),
      StatusCodes.Status201Created => ValueTask.FromResult(Result.ToCreatedOrErrorResponse(Location)),
      StatusCodes.Status202Accepted => ValueTask.FromResult(Result.ToAcceptedOrErrorResponse(Location)),
      StatusCodes.Status205ResetContent => ValueTask.FromResult(Result.ToResponse(SuccessfulResponseType.ResetContent)),
      _ => ValueTask.FromResult(Result.ToResponse()),
    };
  }
}

public class WebResultAtUriOnOk<TValue> : WebResult<TValue> where TValue : notnull
{
  private readonly string? _location;
  public string? Location => _location;

  public WebResultAtUriOnOk(Result<TValue> result, string? uri) : base(result)
  {
    _location = uri;
  }

  public WebResultAtUriOnOk(Result<TValue> result, Uri? uri) : base(result)
  {
    if (uri != null)
    {
      if (uri.IsAbsoluteUri)
      {
        _location = uri.AbsoluteUri;
      }
      else
      {
        _location = uri.GetComponents(UriComponents.SerializationInfoString, UriFormat.UriEscaped);
      }
    }
  }

  public override ValueTask<IResult> ExecuteAsync(HttpContext context, CancellationToken ct)
  {
    if (Result.IsFailed)
    {
      return ValueTask.FromResult(Result.ToErrorResponse());
    }

    var preferredSuccessStatusCodeCache = context.RequestServices
      .GetRequiredKeyedService<IPreferredSuccessStatusCodeCache>(
      WebResultEndpointDefinitions.DefaultPreferredSuccessStatusCodeCacheNameForResultOfT);
    var preferredSuccessCode = preferredSuccessStatusCodeCache
      .GetStatusCode(context);

    return preferredSuccessCode switch
    {
      StatusCodes.Status200OK => ValueTask.FromResult(Result.ToResponse()),
      StatusCodes.Status201Created => ValueTask.FromResult(Result.ToCreatedOrErrorResponse(Location)),
      StatusCodes.Status202Accepted => ValueTask.FromResult(Result.ToAcceptedOrErrorResponse(Location)),
      StatusCodes.Status204NoContent => ValueTask.FromResult(Result.ToResponse(SuccessfulResponseType.NoContent)),
      StatusCodes.Status205ResetContent => ValueTask.FromResult(Result.ToResponse(SuccessfulResponseType.ResetContent)),
      _ => ValueTask.FromResult(Result.ToResponse()),
    };
  }
}
