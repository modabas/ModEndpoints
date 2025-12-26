using ModResults;

namespace ModEndpoints.RemoteServices;

/// <summary>
/// Represents a streaming service endpoint response item.
/// </summary>
public record StreamingResponseItem(
  Result Result,
  string? ResponseType = StreamingResponseItemDefinitions.DefaultResponseType,
  string? ResponseId = null);

/// <summary>
/// Represents a streaming service endpoint response item.
/// </summary>
/// <typeparam name="TResultType">Specifies the type of data payload in the response.</typeparam>
public record StreamingResponseItem<TResultType>(
  Result<TResultType> Result,
  string? ResponseType = StreamingResponseItemDefinitions.DefaultResponseType,
  string? ResponseId = null)
  where TResultType : notnull
{
  public StreamingResponseItem ToItem()
    => new(
      Result: Result,
      ResponseType: ResponseType,
      ResponseId: ResponseId);

  public static implicit operator StreamingResponseItem(StreamingResponseItem<TResultType> itemOfTResponse)
    => itemOfTResponse.ToItem();
}
