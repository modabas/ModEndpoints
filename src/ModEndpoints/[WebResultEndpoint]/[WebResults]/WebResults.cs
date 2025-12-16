using Microsoft.AspNetCore.Routing;
using ModResults;

namespace ModEndpoints;

public static class WebResults
{
  public static WebResult FromResult(Result result)
  {
    return new WebResult(result);
  }

  public static WebResult<TValue> FromResult<TValue>(Result<TValue> result)
    where TValue : notnull
  {
    return new WebResult<TValue>(result);
  }

  public static WebResult<TValue> FromResult<TValue>(TValue value)
    where TValue : notnull
  {
    return new WebResult<TValue>(value);
  }

  public static WebResultAtUriOnOk AtUriOnOk(Result result, string? uri)
  {
    return new WebResultAtUriOnOk(result, uri);
  }

  public static WebResultAtUriOnOk AtUriOnOk(Result result, Uri? uri)
  {
    return new WebResultAtUriOnOk(result, uri);
  }

  public static WebResultAtUriOnOk<TValue> AtUriOnOk<TValue>(Result<TValue> result, string? uri)
    where TValue : notnull
  {
    return new WebResultAtUriOnOk<TValue>(result, uri);
  }

  public static WebResultAtUriOnOk<TValue> AtUriOnOk<TValue>(Result<TValue> result, Uri? uri)
    where TValue : notnull
  {
    return new WebResultAtUriOnOk<TValue>(result, uri);
  }

  public static WebResultAtUriOnOk<TValue> AtUriOnOk<TValue>(TValue value, string? uri)
    where TValue : notnull
  {
    return new WebResultAtUriOnOk<TValue>(value, uri);
  }

  public static WebResultAtUriOnOk<TValue> AtUriOnOk<TValue>(TValue value, Uri? uri)
    where TValue : notnull
  {
    return new WebResultAtUriOnOk<TValue>(value, uri);
  }

  public static WebResultAtRouteOnOk AtRouteOnOk(Result result, string? routeName, object? routeValues)
  {
    return new WebResultAtRouteOnOk(result, routeName, routeValues);
  }

  public static WebResultAtRouteOnOk AtRouteOnOk(Result result, string? routeName, RouteValueDictionary? routeValues)
  {
    return new WebResultAtRouteOnOk(result, routeName, routeValues);
  }

  public static WebResultAtRouteOnOk<TValue> AtRouteOnOk<TValue>(Result<TValue> result, string? routeName, object? routeValues)
    where TValue : notnull
  {
    return new WebResultAtRouteOnOk<TValue>(result, routeName, routeValues);
  }

  public static WebResultAtRouteOnOk<TValue> AtRouteOnOk<TValue>(Result<TValue> result, string? routeName, RouteValueDictionary? routeValues)
    where TValue : notnull
  {
    return new WebResultAtRouteOnOk<TValue>(result, routeName, routeValues);
  }

  public static WebResultAtRouteOnOk<TValue> AtRouteOnOk<TValue>(TValue value, string? routeName, object? routeValues)
    where TValue : notnull
  {
    return new WebResultAtRouteOnOk<TValue>(value, routeName, routeValues);
  }

  public static WebResultAtRouteOnOk<TValue> AtRouteOnOk<TValue>(TValue value, string? routeName, RouteValueDictionary? routeValues)
    where TValue : notnull
  {
    return new WebResultAtRouteOnOk<TValue>(value, routeName, routeValues);
  }
}
