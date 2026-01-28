using Microsoft.AspNetCore.Http;
using ModResults;
using ModResults.MinimalApis;

namespace ModEndpoints;

/// <summary>
/// Represents a web result that writes a stream of server-sent events to the response.
/// </summary>
/// <typeparam name="TValue">The underlying type of the events emitted.</typeparam>
internal sealed class WebResultWithStreamingResponse<TValue> : WebResult<IAsyncEnumerable<TValue>>
{
  public WebResultWithStreamingResponse(Result<IAsyncEnumerable<TValue>> result)
    : base(result)
  {
  }

  public override ValueTask<IResult> ExecuteAsync(HttpContext context, CancellationToken ct)
  {
    if (Result.IsFailed)
    {
      return ValueTask.FromResult(Result.ToErrorResponse());
    }
    return ValueTask.FromResult((IResult)new StreamingResponseResult<TValue>(Result.Value));
  }
}
