using System.Text.Json;
using ModResults;

namespace ModEndpoints.RemoteServices;
public interface IServiceChannelSerializer
{
  Task<Result> DeserializeResultAsync(
    HttpResponseMessage response,
    CancellationToken ct,
    JsonSerializerOptions? jsonSerializerOptions = null);

  Task<Result<T>> DeserializeResultAsync<T>(
    HttpResponseMessage response,
    CancellationToken ct,
    JsonSerializerOptions? jsonSerializerOptions = null) where T : notnull;
}
