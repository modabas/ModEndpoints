using ModEndpoints.RemoteServices.Core;
using ModResults;

namespace ModEndpoints.RemoteServices;
public interface IServiceChannelSerializerForStreamingResponse
{
  IAsyncEnumerable<Result> DeserializeResultAsync(
    HttpResponseMessage response,
    CancellationToken ct);

  IAsyncEnumerable<Result<TResponse>> DeserializeResultAsync<TResponse>(
    HttpResponseMessage response,
    CancellationToken ct)
    where TResponse : notnull;

  ValueTask<HttpContent> CreateContentAsync<TRequest>(
    TRequest request,
    CancellationToken ct)
    where TRequest : IServiceRequestMarker;
}
