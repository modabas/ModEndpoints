using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Text.Json;
using ModEndpoints.RemoteServices.Core;
using ModResults;

namespace ModEndpoints.RemoteServices;

public class DefaultServiceChannelSerializerForStreamingResponse(
  ServiceChannelSerializerOptions options)
  : IServiceChannelSerializerForStreamingResponse
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

  public async IAsyncEnumerable<Result<TResponse>> DeserializeResultAsync<TResponse>(
    HttpResponseMessage response,
    [EnumeratorCancellation] CancellationToken ct)
    where TResponse : notnull
  {
    if (!response.IsSuccessStatusCode)
    {
      yield return Result<TResponse>
        .CriticalError(string.Format(
          string.IsNullOrWhiteSpace(response.ReasonPhrase) ? ResponseNotSuccessfulErrorMessage : ResponseNotSuccessfulWithReasonErrorMessage,
          (int)response.StatusCode,
          response.ReasonPhrase))
        .WithFact(string.Format(
          InstanceFactMessage,
          response.RequestMessage?.Method,
          response.RequestMessage?.RequestUri));
      yield break;
    }
    await foreach (var resultObject in DeserializeResultInternalAsync<Result<TResponse>>(response, ct))
    {
      ct.ThrowIfCancellationRequested();
      yield return resultObject ?? Result<TResponse>
        .CriticalError(DeserializationErrorMessage)
        .WithFact(string.Format(
          InstanceFactMessage,
          response.RequestMessage?.Method,
          response.RequestMessage?.RequestUri));
    }
  }

  public async IAsyncEnumerable<Result> DeserializeResultAsync(
    HttpResponseMessage response,
    [EnumeratorCancellation] CancellationToken ct)
  {
    if (!response.IsSuccessStatusCode)
    {
      yield return Result
        .CriticalError(string.Format(
          string.IsNullOrWhiteSpace(response.ReasonPhrase) ? ResponseNotSuccessfulErrorMessage : ResponseNotSuccessfulWithReasonErrorMessage,
          (int)response.StatusCode,
          response.ReasonPhrase))
        .WithFact(string.Format(
          InstanceFactMessage,
          response.RequestMessage?.Method,
          response.RequestMessage?.RequestUri));
      yield break;
    }
    await foreach (var resultObject in DeserializeResultInternalAsync<Result>(response, ct))
    {
      ct.ThrowIfCancellationRequested();
      yield return resultObject ?? Result
        .CriticalError(DeserializationErrorMessage)
        .WithFact(string.Format(
          InstanceFactMessage,
          response.RequestMessage?.Method,
          response.RequestMessage?.RequestUri));
    }
  }

  private async IAsyncEnumerable<TResult?> DeserializeResultInternalAsync<TResult>(
    HttpResponseMessage response,
    [EnumeratorCancellation] CancellationToken ct)
    where TResult : IModResult
  {
    using (var contentStream = await response.Content.ReadAsStreamAsync(ct))
    //close contentStream forcefully if timeout token is cancelled
    using (ct.Register(() => contentStream.Close()))
    {
      await foreach (var item in JsonSerializer.DeserializeAsyncEnumerable<TResult>(
        contentStream,
        options.DeserializationOptions,
        ct).WithCancellation(ct))
      {
        ct.ThrowIfCancellationRequested();
        yield return item;
      }
    }
  }
}
