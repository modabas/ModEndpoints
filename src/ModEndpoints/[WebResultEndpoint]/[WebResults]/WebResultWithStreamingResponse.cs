#if NET10_0_OR_GREATER
using Microsoft.AspNetCore.Http;
using ModResults;
using ModResults.MinimalApis;

namespace ModEndpoints;

/// <summary>
/// Represents a web result that writes a stream of server-sent events to the response.
/// </summary>
/// <typeparam name="TValue">The underlying type of the events emitted.</typeparam>
internal sealed class WebResultWithStreamingResponse<TValue> : WebResult<IAsyncEnumerable<TValue>>
  where TValue : notnull
{
  private readonly string? _eventType;

  public string? EventType => _eventType;

  public WebResultWithStreamingResponse(Result<IAsyncEnumerable<TValue>> result, string? eventType)
    : base(result)
  {
    _eventType = eventType;
  }

  public override ValueTask<IResult> ExecuteAsync(HttpContext context, CancellationToken ct)
  {
    if (Result.IsFailed)
    {
      return ValueTask.FromResult(Result.ToErrorResponse());
    }
    return ValueTask.FromResult(Results.ServerSentEvents(Result.Value, EventType));
  }
}
#endif
