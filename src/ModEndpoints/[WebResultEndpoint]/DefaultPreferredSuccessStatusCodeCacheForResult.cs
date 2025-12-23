using System.Collections.Concurrent;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Metadata;
using ModEndpoints.Core;

namespace ModEndpoints;

/// <summary>
/// Used to store and retrieve preferred success status codes for Web Result Endpoints without a response model.
/// </summary>
internal sealed class DefaultPreferredSuccessStatusCodeCacheForResult : IPreferredSuccessStatusCodeCache
{
  private readonly int?[] _successStatusCodePriorityList =
  [
    StatusCodes.Status204NoContent,
    StatusCodes.Status200OK,
    StatusCodes.Status201Created,
    StatusCodes.Status202Accepted,
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
    string? endpointName = null;
    //FirstOrDefault to get the first added metadata (in case of multiple)
    var list = endpoint.Metadata.GetOrderedMetadata<EndpointConfigurationMetadata>();
    if (list is not null && list.Count > 0)
    {
      endpointName = list[0].EndpointUniqueName;
    }
    if (string.IsNullOrWhiteSpace(endpointName))
    {
      endpointName = endpoint.ToString();
      if (string.IsNullOrWhiteSpace(endpointName))
      {
        return null;
      }
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
