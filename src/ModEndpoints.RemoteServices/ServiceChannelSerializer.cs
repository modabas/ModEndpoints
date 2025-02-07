using ModResults;
using System.Text.Json;

namespace ModEndpoints.RemoteServices;

public class ServiceChannelSerializer : IServiceChannelSerializer
{
  private const string DeserializationErrorMessage =
    "Cannot deserialize Result object from http response message.";

  private const string ResponseNotSuccessfulErrorMessage =
    "Http response status code does not indicate success: {0}.";

  private const string ResponseNotSuccessfulWithReasonErrorMessage =
    "Http response status code does not indicate success: {0} ({1}).";

  private const string InstanceFactMessage =
    "Instance: {0} {1}";

  public async Task<Result<T>> DeserializeResultAsync<T>(
    HttpResponseMessage response,
    CancellationToken ct,
    JsonSerializerOptions? jsonSerializerOptions = null)
    where T : notnull
  {
    if (!response.IsSuccessStatusCode)
    {
      return Result<T>
        .CriticalError(string.Format(
          string.IsNullOrWhiteSpace(response.ReasonPhrase) ? ResponseNotSuccessfulErrorMessage : ResponseNotSuccessfulWithReasonErrorMessage,
          (int)response.StatusCode,
          response.ReasonPhrase))
        .WithFact(string.Format(
          InstanceFactMessage,
          response.RequestMessage?.Method,
          response.RequestMessage?.RequestUri));
    }
    var resultObject = await DeserializeResultInternalAsync<Result<T>>(response, jsonSerializerOptions, ct);
    return resultObject ?? Result<T>
      .CriticalError(DeserializationErrorMessage)
      .WithFact(string.Format(
        InstanceFactMessage,
        response.RequestMessage?.Method,
        response.RequestMessage?.RequestUri));
  }

  public async Task<Result> DeserializeResultAsync(
    HttpResponseMessage response,
    CancellationToken ct,
    JsonSerializerOptions? jsonSerializerOptions = null)
  {
    if (!response.IsSuccessStatusCode)
    {
      return Result
        .CriticalError(string.Format(
          string.IsNullOrWhiteSpace(response.ReasonPhrase) ? ResponseNotSuccessfulErrorMessage : ResponseNotSuccessfulWithReasonErrorMessage,
          (int)response.StatusCode,
          response.ReasonPhrase))
        .WithFact(string.Format(
          InstanceFactMessage,
          response.RequestMessage?.Method,
          response.RequestMessage?.RequestUri));
    }
    var resultObject = await DeserializeResultInternalAsync<Result>(response, jsonSerializerOptions, ct);
    return resultObject ?? Result
      .CriticalError(DeserializationErrorMessage)
      .WithFact(string.Format(
        InstanceFactMessage,
        response.RequestMessage?.Method,
        response.RequestMessage?.RequestUri));
  }

  private async Task<TResult?> DeserializeResultInternalAsync<TResult>(
    HttpResponseMessage response,
    JsonSerializerOptions? jsonSerializerOptions,
    CancellationToken ct)
    where TResult : IModResult
  {
    using (var contentStream = await response.Content.ReadAsStreamAsync(ct))
    //close contentStream forcefully if timeout token is cancelled
    using (ct.Register(() => contentStream.Close()))
    {
      ct.ThrowIfCancellationRequested();
      return await JsonSerializer.DeserializeAsync<TResult>(
        contentStream,
        jsonSerializerOptions ?? ServiceEndpointDefinitions.DefaultJsonSerializerOptions,
        ct);
    }
  }
}
