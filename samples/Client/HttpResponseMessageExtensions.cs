using System.Text.Json;
using System.Text.Json.Serialization;
using ModResults;

namespace Client;

public static class HttpResponseMessageExtensions
{
  private static readonly JsonSerializerOptions _jsonSerializerOptions = new()
  {
    PropertyNameCaseInsensitive = true,
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    NumberHandling = JsonNumberHandling.AllowReadingFromString
  };

  private const string DeserializationErrorMessage =
    "Cannot deserialize Result object from http response message.";

  private const string ResponseNotSuccessfulErrorMessage =
    "Http response status code does not indicate success: {0}.";

  private const string ResponseNotSuccessfulWithReasonErrorMessage =
    "Http response status code does not indicate success: {0} ({1}).";

  public static async Task<Result<T>> DeserializeResultAsync<T>(
    this HttpResponseMessage response,
    CancellationToken ct)
    where T : notnull
  {
    if (!response.IsSuccessStatusCode)
    {
      return Result<T>
        .CriticalError(string.Format(
          string.IsNullOrWhiteSpace(response.ReasonPhrase) ? ResponseNotSuccessfulErrorMessage : ResponseNotSuccessfulWithReasonErrorMessage,
          (int)response.StatusCode,
          response.ReasonPhrase))
        .WithFact($"Instance: {response.RequestMessage?.Method} {response.RequestMessage?.RequestUri}");
    }
    var resultObject = await response.DeserializeResultInternalAsync<Result<T>>(ct);
    return resultObject ?? Result<T>.Error(DeserializationErrorMessage);
  }

  public static async Task<Result> DeserializeResultAsync(
    this HttpResponseMessage response,
    CancellationToken ct)
  {
    if (!response.IsSuccessStatusCode)
    {
      return Result
        .CriticalError(string.Format(
          string.IsNullOrWhiteSpace(response.ReasonPhrase) ? ResponseNotSuccessfulErrorMessage : ResponseNotSuccessfulWithReasonErrorMessage,
          (int)response.StatusCode,
          response.ReasonPhrase))
        .WithFact($"Instance: {response.RequestMessage?.Method} {response.RequestMessage?.RequestUri}");
    }
    var resultObject = await response.DeserializeResultInternalAsync<Result>(ct);
    return resultObject ?? Result.Error(DeserializationErrorMessage);
  }

  private static async Task<TResult?> DeserializeResultInternalAsync<TResult>(
    this HttpResponseMessage response,
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
        _jsonSerializerOptions,
        ct);
    }
  }
}
