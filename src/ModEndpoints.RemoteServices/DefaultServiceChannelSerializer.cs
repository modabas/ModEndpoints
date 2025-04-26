using System.Net.Http.Json;
using System.Text.Json;
using ModEndpoints.RemoteServices.Core;
using ModResults;

namespace ModEndpoints.RemoteServices;

public class DefaultServiceChannelSerializer(
  ServiceChannelSerializerOptions options)
  : IServiceChannelSerializer
{
  private const string DeserializationErrorMessage =
    "Cannot deserialize Result object from http response message.";

  private const string ResponseNotSuccessfulErrorMessage =
    "Http response status code does not indicate success: {0}.";

  private const string ResponseNotSuccessfulWithReasonErrorMessage =
    "Http response status code does not indicate success: {0} ({1}).";

  private const string InstanceFactMessage =
    "Instance: {0} {1}";

  public ValueTask<HttpContent> CreateContentAsync<TRequest>(
    TRequest request,
    CancellationToken ct)
    where TRequest : IServiceRequestMarker
  {
    return new ValueTask<HttpContent>(
      JsonContent.Create(
        (object)request,
        null,
        options.SerializationOptions));
  }

  public async Task<Result<TResponse>> DeserializeResultAsync<TResponse>(
    HttpResponseMessage response,
    CancellationToken ct)
    where TResponse : notnull
  {
    if (!response.IsSuccessStatusCode)
    {
      return Result<TResponse>
        .CriticalError(string.Format(
          string.IsNullOrWhiteSpace(response.ReasonPhrase) ? ResponseNotSuccessfulErrorMessage : ResponseNotSuccessfulWithReasonErrorMessage,
          (int)response.StatusCode,
          response.ReasonPhrase))
        .WithFact(string.Format(
          InstanceFactMessage,
          response.RequestMessage?.Method,
          response.RequestMessage?.RequestUri));
    }
    var resultObject = await DeserializeResultInternalAsync<Result<TResponse>>(response, ct);
    return resultObject ?? Result<TResponse>
      .CriticalError(DeserializationErrorMessage)
      .WithFact(string.Format(
        InstanceFactMessage,
        response.RequestMessage?.Method,
        response.RequestMessage?.RequestUri));
  }

  public async Task<Result> DeserializeResultAsync(
    HttpResponseMessage response,
    CancellationToken ct)
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
    var resultObject = await DeserializeResultInternalAsync<Result>(response, ct);
    return resultObject ?? Result
      .CriticalError(DeserializationErrorMessage)
      .WithFact(string.Format(
        InstanceFactMessage,
        response.RequestMessage?.Method,
        response.RequestMessage?.RequestUri));
  }

  private async Task<TResult?> DeserializeResultInternalAsync<TResult>(
    HttpResponseMessage response,
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
        options.DeserializationOptions,
        ct);
    }
  }
}
