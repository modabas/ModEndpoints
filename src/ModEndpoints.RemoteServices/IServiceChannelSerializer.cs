using ModEndpoints.RemoteServices.Contracts;
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

  IAsyncEnumerable<StreamingResponseItem> DeserializeStreamingResponseItemAsync(
    HttpResponseMessage response,
    CancellationToken ct);

  IAsyncEnumerable<StreamingResponseItem<TResponse>> DeserializeStreamingResponseItemAsync<TResponse>(
    HttpResponseMessage response,
    CancellationToken ct)
    where TResponse : notnull;

  ValueTask<HttpContent> CreateContentAsync<TRequest>(
    TRequest request,
    CancellationToken ct)
    where TRequest : IServiceRequestMarker;
}
