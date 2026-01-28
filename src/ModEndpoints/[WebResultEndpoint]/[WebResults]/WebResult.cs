using Microsoft.AspNetCore.Http;
using ModResults;

namespace ModEndpoints;

/// <summary>
/// Represents the result of a web operation that can be executed to produce an HTTP response.
/// </summary>
/// <remarks>WebResult serves as an abstraction for encapsulating the outcome of a web request and generating the
/// corresponding HTTP response. Derived types implement the logic for mapping the result to a specific HTTP response,
/// including status codes and error handling. This type is used in WebResultEndpoints to standardize response handling
/// and error reporting.</remarks>
public abstract class WebResult
{
  private readonly Result _result;
  /// <summary>
  /// Get the encapsulated <see cref="Result"/> instance.
  /// </summary>
  public Result Result => _result;
  protected WebResult(Result result)
  {
    _result = result;
  }

  /// <summary>
  /// Executes the result operation asynchronously and returns the appropriate HTTP response.
  /// </summary>
  /// <remarks>The response status code is determined based on the preferred success status code configured in
  /// the request's service provider. If the result indicates failure, an error response is returned.</remarks>
  /// <param name="context">The HTTP context for the current request. Provides access to request and response information.</param>
  /// <param name="ct">A cancellation token that can be used to cancel the asynchronous operation.</param>
  /// <returns>A task that represents the asynchronous execution operation. The task result contains an <see cref="IResult"/>
  /// representing the HTTP response to be sent.</returns>
  public abstract ValueTask<IResult> ExecuteAsync(HttpContext context, CancellationToken ct);

  public static implicit operator WebResult(Result result)
  {
    return new DefaultWebResult(result);
  }
}

/// <summary>
/// Represents the result of a web operation that can be executed to produce an HTTP response.
/// </summary>
/// <typeparam name="TValue"></typeparam>
/// <remarks>WebResult serves as an abstraction for encapsulating the outcome of a web request and generating the
/// corresponding HTTP response. Derived types implement the logic for mapping the result to a specific HTTP response,
/// including status codes and error handling. This type is used in WebResultEndpoints to standardize response handling
/// and error reporting.</remarks>
public abstract class WebResult<TValue>
  where TValue : notnull
{
  private readonly Result<TValue> _result;
  /// <summary>
  /// Get the encapsulated <see cref="Result{TValue}"/> instance.
  /// </summary>
  public Result<TValue> Result => _result;
  protected WebResult(Result<TValue> result)
  {
    _result = result;
  }

  /// <summary>
  /// Executes the result operation asynchronously and returns the appropriate HTTP response.
  /// </summary>
  /// <remarks>The response status code is determined based on the preferred success status code configured in
  /// the request's service provider. If the result indicates failure, an error response is returned.</remarks>
  /// <param name="context">The HTTP context for the current request. Provides access to request and response information.</param>
  /// <param name="ct">A cancellation token that can be used to cancel the asynchronous operation.</param>
  /// <returns>A task that represents the asynchronous execution operation. The task result contains an <see cref="IResult"/>
  /// representing the HTTP response to be sent.</returns>
  public abstract ValueTask<IResult> ExecuteAsync(HttpContext context, CancellationToken ct);

  public static implicit operator WebResult<TValue>(TValue value)
  {
    return new DefaultWebResult<TValue>(value);
  }

  public static implicit operator WebResult<TValue>(Result<TValue> result)
  {
    return new DefaultWebResult<TValue>(result);
  }
}
