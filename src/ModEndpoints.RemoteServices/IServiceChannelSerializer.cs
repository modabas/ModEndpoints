using System.Net.Http.Headers;
using ModEndpoints.RemoteServices.Core;
using ModResults;

namespace ModEndpoints.RemoteServices;
public interface IServiceChannelSerializer
{
  Task<Result> DeserializeResultAsync(
    HttpResponseMessage response,
    CancellationToken ct);

  Task<Result<T>> DeserializeResultAsync<T>(
    HttpResponseMessage response,
    CancellationToken ct)
    where T : notnull;

  ValueTask<HttpContent> CreateContentAsync<T>(
    T request)
    where T : IServiceRequestMarker;
}
