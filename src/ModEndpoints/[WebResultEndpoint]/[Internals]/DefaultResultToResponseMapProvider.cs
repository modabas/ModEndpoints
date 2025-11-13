using System.Collections.Concurrent;
using System.Reflection;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using ModEndpoints.Core;

namespace ModEndpoints;

internal sealed class DefaultResultToResponseMapProvider :
  IResultToResponseMapProvider
{
  private readonly ConcurrentDictionary<Type, string> _mapperNameCache = new();

  public ValueTask<IResultToResponseMapper> GetMapperAsync(
    IWebResultEndpoint endpoint,
    HttpContext context,
    CancellationToken ct)
  {
    var mapperName = GetMapperName(endpoint);
    var mapper = context.RequestServices.GetRequiredKeyedService<IResultToResponseMapper>(mapperName);
    return new ValueTask<IResultToResponseMapper>(mapper);
  }

  private string GetMapperName(IWebResultEndpoint endpoint)
  {
    var mapperName = _mapperNameCache.GetOrAdd(
      endpoint.GetType(),
      (endpointType) => (endpointType.GetCustomAttribute<ResultToResponseMapperAttribute>()?.Name) ??
          WebResultEndpointDefinitions.DefaultResultToResponseMapperName);
    return mapperName;
  }
}
