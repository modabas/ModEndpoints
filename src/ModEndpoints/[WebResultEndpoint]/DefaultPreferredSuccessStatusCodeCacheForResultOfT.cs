using System.Collections.Concurrent;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Metadata;

namespace ModEndpoints;

/// <summary>
/// Used to store and retrieve preferred success status codes for Web Result Endpoints with a response model.
/// </summary>
public class DefaultPreferredSuccessStatusCodeCacheForResultOfT : IPreferredSuccessStatusCodeCache
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

  public int? GetStatusCode(
    HttpContext context)
  {
    var endpoint = context.GetEndpoint();
    if (endpoint is null)
    {
      return null;
    }
    var endpointString = endpoint.ToString();
    if (endpointString is null)
    {
      return null;
    }
    return _cache.GetOrAdd(
      endpointString,
      (_, endpoint) =>
      {
        var producesList = endpoint
          .Metadata
          .GetOrderedMetadata<IProducesResponseTypeMetadata>();
        if (producesList is null || producesList.Count == 0)
        {
          return null;
        }
        return _successStatusCodePriorityList
          .Join(
            producesList,
            outer => outer,
            inner => inner.StatusCode,
            (outer, inner) => outer)
          .FirstOrDefault();
      },
      endpoint);
  }
}
