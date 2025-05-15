using ModResults;

namespace ModEndpoints.RemoteServices;

/// <summary>
/// Represents a streaming service endpoint response item.
/// </summary>
public record StreamingResponseItem
{
  internal readonly string? _responseType;
  public Result Result { get; init; }
  public string ResponseType => _responseType ?? StreamingResponseItemDefinitions.DefaultResponseType;
  public string? ResponseId { get; init; }

  public StreamingResponseItem(
    Result Result,
    string? ResponseType = null,
    string? ResponseId = null)
  {
    this.Result = Result;
    _responseType = ResponseType;
    this.ResponseId = ResponseId;
  }
}

/// <summary>
/// Represents a streaming service endpoint response item.
/// </summary>
/// <typeparam name="TResponse">Specifies the type of data payload in the response.</typeparam>
public record StreamingResponseItem<TResponse>
  where TResponse : notnull
{
  internal readonly string? _responseType;
  public Result<TResponse> Result { get; init; }
  public string ResponseType => _responseType ?? StreamingResponseItemDefinitions.DefaultResponseType;
  public string? ResponseId { get; init; }

  public StreamingResponseItem(
    Result<TResponse> Result,
    string? ResponseType = null,
    string? ResponseId = null)
  {
    this.Result = Result;
    _responseType = ResponseType;
    this.ResponseId = ResponseId;
  }

  public StreamingResponseItem ToItem() 
    => new(
      Result: Result,
      ResponseType: ResponseType,
      ResponseId: ResponseId);

  public static implicit operator StreamingResponseItem(StreamingResponseItem<TResponse> itemOfTResponse)
    => itemOfTResponse.ToItem();
}
