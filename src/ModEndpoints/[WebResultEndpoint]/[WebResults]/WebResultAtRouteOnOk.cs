using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using ModResults;
using ModResults.MinimalApis;

namespace ModEndpoints;

public sealed class WebResultAtRouteOnOk : WebResult
{
  private readonly string? _routeName;
  private readonly RouteValueDictionary? _routeValues;
  public string? RouteName => _routeName;
  public RouteValueDictionary? RouteValues => _routeValues;

  public WebResultAtRouteOnOk(Result result, string? routeName, object? routeValues) : base(result)
  {
    _routeName = routeName;
    _routeValues = new RouteValueDictionary(routeValues);
  }
  public WebResultAtRouteOnOk(Result result, string? routeName, RouteValueDictionary? routeValues) : base(result)
  {
    _routeName = routeName;
    _routeValues = routeValues;
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
      StatusCodes.Status201Created => ValueTask.FromResult(Result.ToCreatedOrErrorResponse(GetLocation(context))),
      StatusCodes.Status202Accepted => ValueTask.FromResult(Result.ToAcceptedOrErrorResponse(GetLocation(context))),
      StatusCodes.Status205ResetContent => ValueTask.FromResult(Result.ToResponse(SuccessfulResponseType.ResetContent)),
      _ => ValueTask.FromResult(Result.ToResponse()),
    };
  }

  private string GetLocation(HttpContext context)
  {
    var linkGenerator = context.RequestServices.GetRequiredService<LinkGenerator>();
    var location = linkGenerator.GetUriByRouteValues(
        context,
        _routeName,
        _routeValues,
        fragment: FragmentString.Empty);
    if (string.IsNullOrEmpty(location))
    {
      throw new InvalidOperationException(
        WebResultEndpointDefinitions.InvalidRouteMessage);
    }
    return location;
  }
}


public sealed class WebResultAtRouteOnOk<TValue> : WebResult<TValue> where TValue : notnull
{
  private readonly string? _routeName;
  private readonly RouteValueDictionary? _routeValues;
  public string? RouteName => _routeName;
  public RouteValueDictionary? RouteValues => _routeValues;

  public WebResultAtRouteOnOk(Result<TValue> result, string? routeName, object? routeValues) : base(result)
  {
    _routeName = routeName;
    _routeValues = new RouteValueDictionary(routeValues);
  }
  public WebResultAtRouteOnOk(Result<TValue> result, string? routeName, RouteValueDictionary? routeValues) : base(result)
  {
    _routeName = routeName;
    _routeValues = routeValues;
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
      StatusCodes.Status201Created => ValueTask.FromResult(Result.ToCreatedOrErrorResponse(GetLocation(context))),
      StatusCodes.Status202Accepted => ValueTask.FromResult(Result.ToAcceptedOrErrorResponse(GetLocation(context))),
      StatusCodes.Status204NoContent => ValueTask.FromResult(Result.ToResponse(SuccessfulResponseType.NoContent)),
      StatusCodes.Status205ResetContent => ValueTask.FromResult(Result.ToResponse(SuccessfulResponseType.ResetContent)),
      _ => ValueTask.FromResult(Result.ToResponse()),
    };
  }

  private string GetLocation(HttpContext context)
  {
    var linkGenerator = context.RequestServices.GetRequiredService<LinkGenerator>();
    var location = linkGenerator.GetUriByRouteValues(
        context,
        _routeName,
        _routeValues,
        fragment: FragmentString.Empty);
    if (string.IsNullOrEmpty(location))
    {
      throw new InvalidOperationException(
        WebResultEndpointDefinitions.InvalidRouteMessage);
    }
    return location;
  }
}
