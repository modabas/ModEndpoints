using ModEndpoints.RemoteServices.Core;
using ModResults;

namespace ModEndpoints.RemoteServices;
public interface IServiceChannelSerializer
{
  Task<Result> DeserializeResultAsync(
    HttpResponseMessage response,
    CancellationToken ct);

  Task<Result<TResponse>> DeserializeResultAsync<TResponse>(
    HttpResponseMessage response,
    CancellationToken ct)
    where TResponse : notnull;

  ValueTask<HttpContent> CreateContentAsync<TRequest>(
    TRequest request,
    CancellationToken ct)
    where TRequest : IServiceRequestMarker;
}
