using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using ModResults;

namespace ModEndpoints;

internal static class WebResultWithLocationRouteOnSuccessExtensions
{
  extension(WebResultWithLocationRouteOnSuccess)
  {
    public static string GetLocation(HttpContext context, string? routeName, RouteValueDictionary? routeValues)
    {
      var linkGenerator = context.RequestServices.GetRequiredService<LinkGenerator>();
      var location = linkGenerator.GetUriByRouteValues(
          context,
          routeName,
          routeValues,
          fragment: FragmentString.Empty);
      if (string.IsNullOrEmpty(location))
      {
        throw new InvalidOperationException(
          WebResultEndpointDefinitions.InvalidRouteMessage);
      }
      return location;
    }
  }
}

public sealed class WebResultWithLocationRouteOnSuccess : WebResult
{
  private readonly string? _routeName;
  private readonly RouteValueDictionary? _routeValues;
  public string? RouteName => _routeName;
  public RouteValueDictionary? RouteValues => _routeValues;

  internal WebResultWithLocationRouteOnSuccess(Result result, string? routeName, object? routeValues) : base(result)
  {
    _routeName = routeName;
    _routeValues = new RouteValueDictionary(routeValues);
  }
  internal WebResultWithLocationRouteOnSuccess(Result result, string? routeName, RouteValueDictionary? routeValues) : base(result)
  {
    _routeName = routeName;
    _routeValues = routeValues;
  }

  public override ValueTask<IResult> ExecuteAsync(HttpContext context, CancellationToken ct)
  {
    return ValueTask.FromResult(this.ExecuteInternal(
      context,
      WebResultWithLocationRouteOnSuccess.GetLocation(
        context,
        _routeName,
        _routeValues)));
  }
}


public sealed class WebResultWithLocationRouteOnSuccess<TValue> : WebResult<TValue> where TValue : notnull
{
  private readonly string? _routeName;
  private readonly RouteValueDictionary? _routeValues;
  public string? RouteName => _routeName;
  public RouteValueDictionary? RouteValues => _routeValues;

  internal WebResultWithLocationRouteOnSuccess(Result<TValue> result, string? routeName, object? routeValues) : base(result)
  {
    _routeName = routeName;
    _routeValues = new RouteValueDictionary(routeValues);
  }
  internal WebResultWithLocationRouteOnSuccess(Result<TValue> result, string? routeName, RouteValueDictionary? routeValues) : base(result)
  {
    _routeName = routeName;
    _routeValues = routeValues;
  }

  public override ValueTask<IResult> ExecuteAsync(HttpContext context, CancellationToken ct)
  {
    return ValueTask.FromResult(this.ExecuteInternal(
      context,
      WebResultWithLocationRouteOnSuccess.GetLocation(
        context,
        _routeName,
        _routeValues)));
  }
}
