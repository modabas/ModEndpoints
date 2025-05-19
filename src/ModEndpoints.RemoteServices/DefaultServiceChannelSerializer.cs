﻿using System.Net.Http.Json;
using System.Runtime.CompilerServices;
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

  public async IAsyncEnumerable<StreamingResponseItem<TResponse>> DeserializeStreamingResponseItemAsync<TResponse>(
    HttpResponseMessage response,
    [EnumeratorCancellation] CancellationToken ct)
    where TResponse : notnull
  {
    if (!response.IsSuccessStatusCode)
    {
      var result = Result<TResponse>
        .CriticalError(string.Format(
          string.IsNullOrWhiteSpace(response.ReasonPhrase) ? ResponseNotSuccessfulErrorMessage : ResponseNotSuccessfulWithReasonErrorMessage,
          (int)response.StatusCode,
          response.ReasonPhrase))
        .WithFact(string.Format(
          InstanceFactMessage,
          response.RequestMessage?.Method,
          response.RequestMessage?.RequestUri));
      yield return new StreamingResponseItem<TResponse>(
        Result: result,
        ResponseType: StreamingResponseItemDefinitions.DefaultClientSideErrorResponseType);
      yield break;
    }
    await foreach (var responseItemObject in DeserializeStreamingResponseItemInternalAsync<StreamingResponseItem<TResponse>>(response, ct))
    {
      ct.ThrowIfCancellationRequested();
      yield return responseItemObject
        ?? new StreamingResponseItem<TResponse>(
          Result: Result<TResponse>
          .CriticalError(DeserializationErrorMessage)
          .WithFact(string.Format(
            InstanceFactMessage,
            response.RequestMessage?.Method,
            response.RequestMessage?.RequestUri)),
          ResponseType: StreamingResponseItemDefinitions.DefaultClientSideErrorResponseType);
    }
  }

  public async IAsyncEnumerable<StreamingResponseItem> DeserializeStreamingResponseItemAsync(
    HttpResponseMessage response,
    [EnumeratorCancellation] CancellationToken ct)
  {
    if (!response.IsSuccessStatusCode)
    {
      var result = Result
        .CriticalError(string.Format(
          string.IsNullOrWhiteSpace(response.ReasonPhrase) ? ResponseNotSuccessfulErrorMessage : ResponseNotSuccessfulWithReasonErrorMessage,
          (int)response.StatusCode,
          response.ReasonPhrase))
        .WithFact(string.Format(
          InstanceFactMessage,
          response.RequestMessage?.Method,
          response.RequestMessage?.RequestUri));
      yield return new StreamingResponseItem(
        Result: result,
        ResponseType: StreamingResponseItemDefinitions.DefaultClientSideErrorResponseType);
      yield break;
    }
    await foreach (var responseItemObject in DeserializeStreamingResponseItemInternalAsync<StreamingResponseItem>(response, ct))
    {
      ct.ThrowIfCancellationRequested();
      yield return responseItemObject
        ?? new StreamingResponseItem(
          Result: Result
          .CriticalError(DeserializationErrorMessage)
          .WithFact(string.Format(
            InstanceFactMessage,
            response.RequestMessage?.Method,
            response.RequestMessage?.RequestUri)),
          ResponseType: StreamingResponseItemDefinitions.DefaultClientSideErrorResponseType);
    }
  }

  private async IAsyncEnumerable<TResult?> DeserializeStreamingResponseItemInternalAsync<TResult>(
    HttpResponseMessage response,
    [EnumeratorCancellation] CancellationToken ct)
  {
    using (var contentStream = await response.Content.ReadAsStreamAsync(ct))
    //close contentStream forcefully if timeout token is cancelled
    using (ct.Register(() => contentStream.Close()))
    {
      await foreach (var item in JsonSerializer.DeserializeAsyncEnumerable<TResult>(
        contentStream,
        options.StreamingDeserializationOptions,
        ct).WithCancellation(ct))
      {
        ct.ThrowIfCancellationRequested();
        yield return item;
      }
    }
  }
}
