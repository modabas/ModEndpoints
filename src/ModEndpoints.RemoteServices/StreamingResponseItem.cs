using ModResults;

namespace ModEndpoints.RemoteServices;

/// <summary>
/// Represents a streaming service endpoint response item.
/// </summary>
public record StreamingResponseItem(
  Result Result,
  string? ResponseType = null,
  string? ResponseId = null)
{
  public static implicit operator StreamingResponseItem(Result result) 
    => new(Result: result);
}

/// <summary>
/// Represents a streaming service endpoint response item.
/// </summary>
/// <typeparam name="TResponse">Specifies the type of data payload in the response.</typeparam>
public record StreamingResponseItem<TResponse>(
  Result<TResponse> Result,
  string? ResponseType = null,
  string? ResponseId = null)
  where TResponse : notnull
{
  public StreamingResponseItem ToSrItem() 
    => new(
      Result: Result,
      ResponseType: ResponseType,
      ResponseId: ResponseId);

  public static implicit operator StreamingResponseItem<TResponse>(TResponse responseValue)
    => new(Result: responseValue);

  public static implicit operator StreamingResponseItem<TResponse>(Result<TResponse> result)
    => new(Result: result);

  public static implicit operator StreamingResponseItem(StreamingResponseItem<TResponse> itemOfTResponse)
    => itemOfTResponse.ToSrItem();
}
