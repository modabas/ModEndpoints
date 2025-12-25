using System.Collections.Concurrent;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Metadata;
using ModEndpoints.Core;

namespace ModEndpoints;

/// <summary>
/// Used to store and retrieve preferred success status codes for Web Result Endpoints with a response model.
/// </summary>
internal sealed class DefaultPreferredSuccessStatusCodeCacheForResultOfT : IPreferredSuccessStatusCodeCache
{
  private readonly int?[] _successStatusCodePriorityList =
  [
    StatusCodes.Status200OK,
    StatusCodes.Status201Created,
    StatusCodes.Status202Accepted,
    StatusCodes.Status204NoContent,
    StatusCodes.Status205ResetContent
  ];

  private readonly ConcurrentDictionary<string, int?> _cache = new();
  private readonly IEndpointNameResolver _endpointNameResolver;

  public DefaultPreferredSuccessStatusCodeCacheForResultOfT(
    IEndpointNameResolver endpointNameResolver)
  {
    _endpointNameResolver = endpointNameResolver;
  }

  public int? GetStatusCode(
    HttpContext context)
  {
    var endpoint = context.GetEndpoint();
    if (endpoint is null)
    {
      return null;
    }
    var endpointName = _endpointNameResolver.GetName(endpoint);
    if (string.IsNullOrWhiteSpace(endpointName))
    {
      return null;
    }
    return _cache.GetOrAdd(
      endpointName,
      (_, state) =>
      {
        var producesList = state.Endpoint
          .Metadata
          .GetOrderedMetadata<IProducesResponseTypeMetadata>();
#if NET9_0
        // Exclude IResult responses with application/json content type for StatusCodes.Status200OK
        // .NET 9.0 adds it by default, so we filter it out while determining preferred status code
        // see https://github.com/dotnet/aspnetcore/issues/57801
        producesList = producesList.Where(p =>
          (p.StatusCode == StatusCodes.Status200OK &&
          p.ContentTypes.Count() == 1 &&
          p.ContentTypes.First() == "application/json" &&
          p.Type == typeof(IResult)) == false)
        .ToList()
        .AsReadOnly();
#endif
        if (producesList.Count == 0)
        {
          return null;
        }
        return state.StatusCodes
          .Join(
            producesList,
            outer => outer,
            inner => inner.StatusCode,
            (outer, inner) => outer)
          .FirstOrDefault();
      },
      new { Endpoint = endpoint, StatusCodes = _successStatusCodePriorityList });
  }
}
