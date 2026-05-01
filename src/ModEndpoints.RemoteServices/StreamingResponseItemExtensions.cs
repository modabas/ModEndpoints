using ModResults;

namespace ModEndpoints.RemoteServices;

public static class StreamingResponseItemExtensions
{
  extension(StreamingResponseItem)
  {
    /// <summary>
    /// Creates a new instance of the <see cref="StreamingResponseItem"/> class using the specified result, response type, and
    /// response identifier.
    /// </summary>
    /// <param name="result">The result to include in the streaming response item. Cannot be null.</param>
    /// <param name="responseType">The response type identifier to associate with the item. If not specified, the default response type is used.</param>
    /// <param name="responseId">An optional identifier for the response. If null, no identifier is set.</param>
    /// <returns>A StreamingResponseItem instance containing the specified result, response type, and response identifier.</returns>
    public static StreamingResponseItem FromResult(
      Result result,
      string? responseType = StreamingResponseItemDefinitions.DefaultResponseType,
      string? responseId = null) => new(
        Result: result,
        ResponseType: responseType,
        ResponseId: responseId
      );

    /// <summary>
    /// Creates a new instance of the <see cref="StreamingResponseItem{TResultValue}"/> class using the specified result, response type, and
    /// response identifier.
    /// </summary>
    /// <typeparam name="TResultValue">The type of the value contained in the result. Must be non-nullable.</typeparam>
    /// <param name="result">The result to include in the streaming response item. Cannot be null.</param>
    /// <param name="responseType">The response type identifier to associate with the item. If not specified, the default response type is used.</param>
    /// <param name="responseId">An optional identifier for the response item. If null, no identifier is set.</param>
    /// <returns>A <see cref="StreamingResponseItem{TResultValue}"/> containing the specified result, response type, and response identifier.</returns>
    public static StreamingResponseItem<TResultValue> FromResult<TResultValue>(
      Result<TResultValue> result,
      string? responseType = StreamingResponseItemDefinitions.DefaultResponseType,
      string? responseId = null)
      where TResultValue : notnull => new(
        Result: result,
        ResponseType: responseType,
        ResponseId: responseId
      );

    /// <summary>
    /// Creates a new instance of the <see cref="StreamingResponseItem{TResultValue}"/> class using the specified result value, response type, and
    /// response identifier.
    /// </summary>
    /// <typeparam name="TResultValue">The type of the value contained in the result. Must be non-nullable.</typeparam>
    /// <param name="value">The result value to include in the streaming response item. Cannot be null.</param>
    /// <param name="responseType">The response type identifier to associate with the item. If not specified, the default response type is used.</param>
    /// <param name="responseId">An optional identifier for the response item. If null, no identifier is set.</param>
    /// <returns>A <see cref="StreamingResponseItem{TResultValue}"/> containing the specified result value, response type, and response identifier.</returns>
    public static StreamingResponseItem<TResultValue> FromResult<TResultValue>(
      TResultValue value,
      string? responseType = StreamingResponseItemDefinitions.DefaultResponseType,
      string? responseId = null)
      where TResultValue : notnull => new(
        Result: Result.Ok(value),
        ResponseType: responseType,
        ResponseId: responseId
      );
  }
}
