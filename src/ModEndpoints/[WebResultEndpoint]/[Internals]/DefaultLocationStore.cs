using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace ModEndpoints;

internal sealed class DefaultLocationStore(IHttpContextAccessor contextAccessor) : ILocationStore
{
  private string? _value;

  public ValueTask<string?> GetValueAsync(
    CancellationToken ct)
  {
    return new ValueTask<string?>(_value);
  }

  public ValueTask SetValueAsync(
    string? uri,
    CancellationToken ct)
  {
    _value = uri;
    return ValueTask.CompletedTask;
  }

  public ValueTask SetValueAsync(
    Uri? uri,
    CancellationToken ct)
  {
    if (uri != null)
    {
      if (uri.IsAbsoluteUri)
      {
        _value = uri.AbsoluteUri;
      }
      else
      {
        _value = uri.GetComponents(UriComponents.SerializationInfoString, UriFormat.UriEscaped);
      }
    }
    return ValueTask.CompletedTask;
  }

  public ValueTask SetValueAsync(
    string? routeName,
    object? routeValues,
    CancellationToken ct)
  {
    var context = contextAccessor.HttpContext;
    if (context is null)
    {
      throw new InvalidOperationException(
        WebResultEndpointDefinitions.HttpContextIsInvalidMessage);
    }
    var linkGenerator = context.RequestServices.GetRequiredService<LinkGenerator>();
    var url = linkGenerator.GetUriByRouteValues(
        context,
        routeName,
        routeValues,
        fragment: FragmentString.Empty);
    if (string.IsNullOrEmpty(url))
    {
      throw new InvalidOperationException(
        WebResultEndpointDefinitions.InvalidRouteMessage);
    }
    _value = url;
    return ValueTask.CompletedTask;
  }
}
