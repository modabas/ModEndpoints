using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace ModEndpoints;

/// <summary>
/// Represents a result that writes a stream of events to the response.
/// </summary>
/// <typeparam name="T">The underlying type of the events emitted.</typeparam>
internal sealed class StreamingResponseResult<T> : IResult, IStatusCodeHttpResult, IValueHttpResult, IValueHttpResult<IAsyncEnumerable<T>>
{
  private static readonly byte[] _start = "["u8.ToArray();
  private static readonly byte[] _end = "]"u8.ToArray();
  private static readonly byte[] _seperator = ","u8.ToArray();
  private readonly IAsyncEnumerable<T> _events;

  public IAsyncEnumerable<T>? Value => _events;
  object? IValueHttpResult.Value => Value;

  public int StatusCode => StatusCodes.Status200OK;
  int? IStatusCodeHttpResult.StatusCode => StatusCode;

  public StreamingResponseResult(IAsyncEnumerable<T> events)
  {
    _events = events;
  }

  public async Task ExecuteAsync(HttpContext httpContext)
  {
    httpContext.Response.ContentType = "application/json; charset=utf-8";
    httpContext.Response.Headers.CacheControl = "no-cache,no-store";
    httpContext.Response.Headers.Pragma = "no-cache";

    var bufferingFeature = httpContext.Features.GetRequiredFeature<IHttpResponseBodyFeature>();
    bufferingFeature.DisableBuffering();

    var jsonOptions = httpContext.RequestServices.GetService<IOptions<JsonOptions>>()?.Value ?? new JsonOptions();

    bool isFirstItem = true;
    var ct = httpContext.RequestAborted;

    httpContext.Response.StatusCode = StatusCode;

    await httpContext.Response.Body.WriteAsync(_start, ct).ConfigureAwait(false);
    await foreach (var @event in _events.WithCancellation(ct))
    {
      if (!isFirstItem)
      {
        await httpContext.Response.Body.WriteAsync(_seperator, ct).ConfigureAwait(false);
      }
      await WriteAsJsonAsync(@event, httpContext.Response.Body, jsonOptions, ct).ConfigureAwait(false);
      isFirstItem = false;
    }
    await httpContext.Response.Body.WriteAsync(_end, ct).ConfigureAwait(false);
  }

  private static async Task WriteAsJsonAsync(T item, Stream writer, JsonOptions jsonOptions, CancellationToken ct)
  {
    if (item is null)
    {
      var nullBytes = JsonSerializer.SerializeToUtf8Bytes<object?>(null, jsonOptions.SerializerOptions);
      await writer.WriteAsync(nullBytes, ct).ConfigureAwait(false);
      return;
    }

    var runtimeType = item.GetType();
    var jsonTypeInfo = jsonOptions.SerializerOptions.GetTypeInfo(typeof(T));

    var typeInfo = jsonTypeInfo.ShouldUseWith(runtimeType)
        ? jsonTypeInfo
        : jsonOptions.SerializerOptions.GetTypeInfo(typeof(object));

    var json = JsonSerializer.SerializeToUtf8Bytes(item, typeInfo);
    await writer.WriteAsync(json, ct).ConfigureAwait(false);
  }
}
