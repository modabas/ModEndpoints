using Microsoft.AspNetCore.Http;
using ModEndpoints.Core;

namespace ModEndpoints;

/// <summary>
/// Helper methods for result to response mappers.
/// </summary>
public interface IResultToResponseMapProvider
{
  /// <summary>
  /// Gets the mapper for a particular <see cref="IWebResultEndpoint"/> implementation,
  /// that will be used to map a handler result type instance to <see cref="IResult"/>. 
  /// </summary>
  /// <param name="endpoint"></param>
  /// <param name="context"></param>
  /// <param name="ct"></param>
  /// <returns></returns>
  ValueTask<IResultToResponseMapper> GetMapperAsync(IWebResultEndpoint endpoint, HttpContext context, CancellationToken ct);
}
