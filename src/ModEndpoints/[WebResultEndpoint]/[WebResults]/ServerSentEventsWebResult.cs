#if NET10_0_OR_GREATER
using System.Net.ServerSentEvents;
using Microsoft.AspNetCore.Http;
using ModResults;
using ModResults.MinimalApis;

namespace ModEndpoints;

/// <summary>
/// Represents a web result that writes a stream of server-sent events to the response.
/// </summary>
/// <typeparam name="TValue">The underlying type of the events emitted.</typeparam>
internal sealed class ServerSentEventsWebResult<TValue> : WebResult<IAsyncEnumerable<SseItem<TValue>>>
{
  private ServerSentEventsWebResult(Result<IAsyncEnumerable<SseItem<TValue>>> result) : base(result)
  {
  }

  public static ServerSentEventsWebResult<TValue> Create(Result<IAsyncEnumerable<SseItem<TValue>>> result)
  {
    return new ServerSentEventsWebResult<TValue>(result);
  }

  public static ServerSentEventsWebResult<TValue> Create(Result<IAsyncEnumerable<TValue>> result, string? eventType)
  {
    return new ServerSentEventsWebResult<TValue>(WrapEvents(result, eventType));
  }

  private static Result<IAsyncEnumerable<SseItem<TValue>>> WrapEvents(Result<IAsyncEnumerable<TValue>> result, string? eventType)
  {
    return result.ToResult(
      static (value, state) => WrapEventsAsync(value, state.eventType),
      new { eventType });
  }

  private static async IAsyncEnumerable<SseItem<TValue>> WrapEventsAsync(IAsyncEnumerable<TValue> events, string? eventType)
  {
    await foreach (var item in events.ConfigureAwait(false))
    {
      yield return new SseItem<TValue>(item, eventType);
    }
  }

  public override ValueTask<IResult> ExecuteAsync(HttpContext context, CancellationToken ct)
  {
    if (Result.IsFailed)
    {
      return ValueTask.FromResult(Result.ToErrorResponse());
    }
    return ValueTask.FromResult(Results.ServerSentEvents(Result.Value));
  }
}
#endif
