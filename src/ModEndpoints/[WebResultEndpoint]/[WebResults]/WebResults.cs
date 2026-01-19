using Microsoft.AspNetCore.Routing;
using ModResults;

namespace ModEndpoints;

public static class WebResults
{
  #region "FromResult"
  /// <summary>
  /// Creates a new instance of the <see cref="DefaultWebResult"/> class from the specified Result object.
  /// </summary>
  /// <param name="result">The Result object to wrap in a WebResult. Cannot be null.</param>
  /// <returns>A <see cref="WebResult"/> instance that encapsulates the specified Result object.</returns>
  public static WebResult FromResult(Result result)
  {
    return new DefaultWebResult(result);
  }

  /// <summary>
  /// Creates a new <see cref="DefaultWebResult{TValue}"/> instance from the specified result.
  /// </summary>
  /// <typeparam name="TValue">The type of the value contained in the result. Must not be null.</typeparam>
  /// <param name="result">The result to wrap in a <see cref="WebResult{TValue}"/>. Cannot be null.</param>
  /// <returns>A <see cref="WebResult{TValue}"/> that encapsulates the specified result.</returns>
  public static WebResult<TValue> FromResult<TValue>(Result<TValue> result)
    where TValue : notnull
  {
    return new DefaultWebResult<TValue>(result);
  }

  /// <summary>
  /// Creates a new <see cref="DefaultWebResult{TValue}"/> that represents a successful result containing the specified value.
  /// </summary>
  /// <typeparam name="TValue">The type of the value to be wrapped in the result. Must be non-null.</typeparam>
  /// <param name="value">The value to include in the successful result. Cannot be null.</param>
  /// <returns>A <see cref="WebResult{TValue}"/> instance containing the specified value as a successful result.</returns>
  public static WebResult<TValue> FromResult<TValue>(TValue value)
    where TValue : notnull
  {
    return new DefaultWebResult<TValue>(value);
  }
  #endregion

  #region "WithLocationUriOnSuccess"

  /// <summary>
  /// Creates a new <see cref="WebResultWithLocationUriOnSuccess"/> instance that returns
  /// a `Location` header set to provided Uri when mapped HTTP response is either `201 Created` or `202 Accepted`.
  /// Otherwise, it behaves like <see cref="DefaultWebResult"/>.
  /// </summary>
  /// <param name="result">The result to be encapsulated in the response. Represents the outcome of the operation.</param>
  /// <param name="uri">The URI to associate with the response if the operation is successful. Can be null if no URI should be set.</param>
  /// <returns>A <see cref="WebResult"/> containing the specified result and associated URI.</returns>
  public static WebResult WithLocationUriOnSuccess(Result result, string? uri)
  {
    return new WebResultWithLocationUriOnSuccess(result, uri);
  }

  /// <summary>
  /// Creates a new <see cref="WebResultWithLocationUriOnSuccess"/> instance that returns
  /// a `Location` header set to provided Uri when mapped HTTP response is either `201 Created` or `202 Accepted`.
  /// Otherwise, it behaves like <see cref="DefaultWebResult"/>.
  /// </summary>
  /// <param name="result">The result to be encapsulated in the response. Represents the outcome of the operation.</param>
  /// <param name="uri">The URI to associate with the response if the operation is successful. Can be null if no URI should be set.</param>
  /// <returns>A <see cref="WebResult"/> containing the specified result and associated URI.</returns>
  public static WebResult WithLocationUriOnSuccess(Result result, Uri? uri)
  {
    return new WebResultWithLocationUriOnSuccess(result, uri);
  }

  /// <summary>
  /// Creates a new <see cref="WebResultWithLocationUriOnSuccess{TValue}"/> instance that returns
  /// a `Location` header set to provided Uri when mapped HTTP response is either `201 Created` or `202 Accepted`.
  /// Otherwise, it behaves like <see cref="DefaultWebResult{TValue}"/>.
  /// </summary>
  /// <typeparam name="TValue"></typeparam>
  /// <param name="result">The result to be encapsulated in the response. Represents the outcome of the operation.</param>
  /// <param name="uri">The URI to associate with the response if the operation is successful. Can be null if no URI should be set.</param>
  /// <returns>A <see cref="WebResult{TValue}"/> containing the specified result and associated URI.</returns>
  public static WebResult<TValue> WithLocationUriOnSuccess<TValue>(Result<TValue> result, string? uri)
    where TValue : notnull
  {
    return new WebResultWithLocationUriOnSuccess<TValue>(result, uri);
  }

  /// <summary>
  /// Creates a new <see cref="WebResultWithLocationUriOnSuccess{TValue}"/> instance that returns
  /// a `Location` header set to provided Uri when mapped HTTP response is either `201 Created` or `202 Accepted`.
  /// Otherwise, it behaves like <see cref="DefaultWebResult{TValue}"/>.
  /// </summary>
  /// <typeparam name="TValue"></typeparam>
  /// <param name="result">The result to be encapsulated in the response. Represents the outcome of the operation.</param>
  /// <param name="uri">The URI to associate with the response if the operation is successful. Can be null if no URI should be set.</param>
  /// <returns>A <see cref="WebResult{TValue}"/> containing the specified result and associated URI.</returns>
  public static WebResult<TValue> WithLocationUriOnSuccess<TValue>(Result<TValue> result, Uri? uri)
    where TValue : notnull
  {
    return new WebResultWithLocationUriOnSuccess<TValue>(result, uri);
  }

  /// <summary>
  /// Creates a new <see cref="WebResultWithLocationUriOnSuccess{TValue}"/> instance that returns
  /// a `Location` header set to provided Uri when mapped HTTP response is either `201 Created` or `202 Accepted`.
  /// Otherwise, it behaves like <see cref="DefaultWebResult{TValue}"/>.
  /// </summary>
  /// <typeparam name="TValue">The type of the value to include in the response. Must not be null.</typeparam>
  /// <param name="value">The value to include in the successful result. Cannot be null.</param>
  /// <param name="uri">The URI to set in the Location header of the response. Can be null if no Location header is required.</param>
  /// <returns>A successful <see cref="WebResult{TValue}"/> containing the specified value and Location URI.</returns>
  public static WebResult<TValue> WithLocationUriOnSuccess<TValue>(TValue value, string? uri)
    where TValue : notnull
  {
    return new WebResultWithLocationUriOnSuccess<TValue>(value, uri);
  }

  /// <summary>
  /// Creates a new <see cref="WebResultWithLocationUriOnSuccess{TValue}"/> instance that returns
  /// a `Location` header set to provided Uri when mapped HTTP response is either `201 Created` or `202 Accepted`.
  /// Otherwise, it behaves like <see cref="DefaultWebResult{TValue}"/>.
  /// </summary>
  /// <typeparam name="TValue">The type of the value to include in the response. Must not be null.</typeparam>
  /// <param name="value">The value to include in the successful result. Cannot be null.</param>
  /// <param name="uri">The URI to set in the Location header of the response. Can be null if no Location header is required.</param>
  /// <returns>A successful <see cref="WebResult{TValue}"/> containing the specified value and Location URI.</returns>
  public static WebResult<TValue> WithLocationUriOnSuccess<TValue>(TValue value, Uri? uri)
    where TValue : notnull
  {
    return new WebResultWithLocationUriOnSuccess<TValue>(value, uri);
  }
  #endregion

  #region "WithLocationRouteOnSuccess"

  /// <summary>
  /// Creates a new <see cref="WebResultWithLocationRouteOnSuccess"/> instance that returns
  /// a `Location` header based on a named route when mapped HTTP response is either `201 Created` or `202 Accepted`.
  /// Otherwise, it behaves like <see cref="DefaultWebResult"/>.
  /// </summary>
  /// <param name="result">The result containing the value to include in the response. Must not be null.</param>
  /// <param name="routeName">The name of the route to use for generating the Location header.</param>
  /// <param name="routeValues">An object containing the route values to use for URL generation.</param>
  /// <returns>A <see cref="WebResult"/> containing the specified result and associated URI.</returns>
  public static WebResult WithLocationRouteOnSuccess(Result result, string? routeName, object? routeValues)
  {
    return new WebResultWithLocationRouteOnSuccess(result, routeName, routeValues);
  }

  /// <summary>
  /// Creates a new <see cref="WebResultWithLocationRouteOnSuccess"/> instance that returns
  /// a `Location` header based on a named route when mapped HTTP response is either `201 Created` or `202 Accepted`.
  /// Otherwise, it behaves like <see cref="DefaultWebResult"/>.
  /// </summary>
  /// <param name="result">The result containing the value to include in the response. Must not be null.</param>
  /// <param name="routeName">The name of the route to use for generating the Location header.</param>
  /// <param name="routeValues">An object containing the route values to use for URL generation.</param>
  /// <returns>A <see cref="WebResult"/> containing the specified result and associated URI.</returns>
  public static WebResult WithLocationRouteOnSuccess(Result result, string? routeName, RouteValueDictionary? routeValues)
  {
    return new WebResultWithLocationRouteOnSuccess(result, routeName, routeValues);
  }

  /// <summary>
  /// Creates a new <see cref="WebResultWithLocationRouteOnSuccess{TValue}"/> instance that returns
  /// a `Location` header based on a named route when mapped HTTP response is either `201 Created` or `202 Accepted`.
  /// Otherwise, it behaves like <see cref="DefaultWebResult{TValue}"/>.
  /// </summary>
  /// <typeparam name="TValue">The type of the value contained in the result. Must be non-null.</typeparam>
  /// <param name="result">The result containing the value to include in the response. Must not be null.</param>
  /// <param name="routeName">The name of the route to use for generating the Location header.</param>
  /// <param name="routeValues">An object containing the route values to use for URL generation.</param>
  /// <returns>A <see cref="WebResult{TValue}"/> containing the specified result and associated URI.</returns>
  public static WebResult<TValue> WithLocationRouteOnSuccess<TValue>(Result<TValue> result, string? routeName, object? routeValues)
    where TValue : notnull
  {
    return new WebResultWithLocationRouteOnSuccess<TValue>(result, routeName, routeValues);
  }

  /// <summary>
  /// Creates a new <see cref="WebResultWithLocationRouteOnSuccess{TValue}"/> instance that returns
  /// a `Location` header based on a named route when mapped HTTP response is either `201 Created` or `202 Accepted`.
  /// Otherwise, it behaves like <see cref="DefaultWebResult{TValue}"/>.
  /// </summary>
  /// <typeparam name="TValue">The type of the value contained in the result. Must be non-null.</typeparam>
  /// <param name="result">The result containing the value to include in the response. Must not be null.</param>
  /// <param name="routeName">The name of the route to use for generating the Location header.</param>
  /// <param name="routeValues">An object containing the route values to use for URL generation.</param>
  /// <returns>A <see cref="WebResult{TValue}"/> containing the specified result and associated URI.</returns>
  public static WebResult<TValue> WithLocationRouteOnSuccess<TValue>(Result<TValue> result, string? routeName, RouteValueDictionary? routeValues)
    where TValue : notnull
  {
    return new WebResultWithLocationRouteOnSuccess<TValue>(result, routeName, routeValues);
  }

  /// <summary>
  /// Creates a new <see cref="WebResultWithLocationRouteOnSuccess{TValue}"/> instance that returns
  /// a `Location` header based on a named route when mapped HTTP response is either `201 Created` or `202 Accepted`.
  /// Otherwise, it behaves like <see cref="DefaultWebResult{TValue}"/>.
  /// </summary>
  /// <typeparam name="TValue">The type of the value contained in the result. Must be non-null.</typeparam>
  /// <param name="value">The value to include in the successful result. Cannot be null.</param>
  /// <param name="routeName">The name of the route to use for generating the Location header.</param>
  /// <param name="routeValues">An object containing the route values to use for URL generation.</param>
  /// <returns>A successful <see cref="WebResult{TValue}"/> containing the specified value and associated URI.</returns>
  public static WebResult<TValue> WithLocationRouteOnSuccess<TValue>(TValue value, string? routeName, object? routeValues)
    where TValue : notnull
  {
    return new WebResultWithLocationRouteOnSuccess<TValue>(value, routeName, routeValues);
  }

  /// <summary>
  /// Creates a new <see cref="WebResultWithLocationRouteOnSuccess{TValue}"/> instance that returns
  /// a `Location` header based on a named route when mapped HTTP response is either `201 Created` or `202 Accepted`.
  /// Otherwise, it behaves like <see cref="DefaultWebResult{TValue}"/>.
  /// </summary>
  /// <typeparam name="TValue">The type of the value contained in the result. Must be non-null.</typeparam>
  /// <param name="value">The value to include in the successful result. Cannot be null.</param>
  /// <param name="routeName">The name of the route to use for generating the Location header.</param>
  /// <param name="routeValues">An object containing the route values to use for URL generation.</param>
  /// <returns>A successful <see cref="WebResult{TValue}"/> containing the specified value and associated URI.</returns>
  public static WebResult<TValue> WithLocationRouteOnSuccess<TValue>(TValue value, string? routeName, RouteValueDictionary? routeValues)
    where TValue : notnull
  {
    return new WebResultWithLocationRouteOnSuccess<TValue>(value, routeName, routeValues);
  }
  #endregion

  #region "ServerSentEvents"
#if NET10_0_OR_GREATER
  /// <summary>
  /// Creates a web result that streams server-sent events from the specified result containing asynchronous event sequence.
  /// </summary>
  /// <remarks>Use this method to enable real-time, push-based communication from the server to the client over
  /// HTTP using the Server-Sent Events (SSE) protocol. The response is formatted as an SSE stream, allowing clients to
  /// receive events as they are emitted by the server.</remarks>
  /// <typeparam name="TValue">The type of the data payload contained in each server-sent event. Must not be null.</typeparam>
  /// <param name="result">A result containing an asynchronous sequence of server-sent event items to be streamed to the client.</param>
  /// <returns>A web result that streams the provided events to the client using the server-sent events protocol.</returns>
  public static WebResult<IAsyncEnumerable<System.Net.ServerSentEvents.SseItem<TValue>>> ServerSentEvents<TValue>(
    Result<IAsyncEnumerable<System.Net.ServerSentEvents.SseItem<TValue>>> result)
    where TValue : notnull
  {
    return ServerSentEventsWebResult<TValue>.Create(result);
  }

  /// <summary>
  /// Creates a web result that streams server-sent events to the client using the specified asynchronous event
  /// sequence.
  /// </summary>
  /// <remarks>Use this method to enable real-time, push-based communication from the server to the client over
  /// HTTP using the Server-Sent Events (SSE) protocol. The response is formatted as an SSE stream, allowing clients to
  /// receive events as they are emitted by the server.</remarks>
  /// <typeparam name="TValue">The type of the data payload contained in each server-sent event. Must not be null.</typeparam>
  /// <param name="events">An asynchronous sequence of server-sent event items to be streamed to the client. Cannot be null.</param>
  /// <returns>A web result that streams the provided events to the client using the server-sent events protocol.</returns>
  public static WebResult<IAsyncEnumerable<System.Net.ServerSentEvents.SseItem<TValue>>> ServerSentEvents<TValue>(
    IAsyncEnumerable<System.Net.ServerSentEvents.SseItem<TValue>> events)
    where TValue : notnull
  {
    return ServerSentEventsWebResult<TValue>.Create(Result<IAsyncEnumerable<System.Net.ServerSentEvents.SseItem<TValue>>>.Ok(events));
  }

  /// <summary>
  /// Creates a web result that streams server-sent events (SSE) from the specified result containing an asynchronous sequence of event data items.
  /// </summary>
  /// <remarks>Use this method to enable real-time, push-based communication from the server to the client over
  /// HTTP using the Server-Sent Events (SSE) protocol. The response is formatted as an SSE stream, allowing clients to
  /// receive events as they are emitted by the server.</remarks>
  /// <typeparam name="TValue">The type of the data payload contained in each server-sent event. Must not be null.</typeparam>
  /// <param name="result">A result containing an asynchronous sequence of event data items to be streamed to the client.</param>
  /// <param name="eventType">The event type to associate with each server-sent event. If null, the default event type is used.</param>
  /// <returns>A web result that streams the provided events to the client using the server-sent events protocol.</returns>
  public static WebResult<IAsyncEnumerable<System.Net.ServerSentEvents.SseItem<TValue>>> ServerSentEvents<TValue>(
    Result<IAsyncEnumerable<TValue>> result, string? eventType = null)
    where TValue : notnull
  {
    return ServerSentEventsWebResult<TValue>.Create(result, eventType);
  }

  /// <summary>
  /// Creates a web result that streams server-sent events (SSE) from the specified asynchronous sequence.
  /// </summary>
  /// <remarks>Use this method to enable real-time, push-based communication from the server to the client over
  /// HTTP using the Server-Sent Events (SSE) protocol. The response is formatted as an SSE stream, allowing clients to
  /// receive events as they are emitted by the server.</remarks>
  /// <typeparam name="TValue">The type of the data payload contained in each server-sent event. Must not be null.</typeparam>
  /// <param name="events">An asynchronous sequence of event data items to be sent to the client as server-sent events. Cannot be null.</param>
  /// <param name="eventType">The event type to associate with each server-sent event. If null, the default event type is used.</param>
  /// <returns>A web result that streams the provided events to the client using the server-sent events protocol.</returns>
  public static WebResult<IAsyncEnumerable<System.Net.ServerSentEvents.SseItem<TValue>>> ServerSentEvents<TValue>(
    IAsyncEnumerable<TValue> events, string? eventType = null)
    where TValue : notnull
  {
    return ServerSentEventsWebResult<TValue>.Create(Result<IAsyncEnumerable<TValue>>.Ok(events), eventType);
  }
#endif
  #endregion
}
