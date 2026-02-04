using Microsoft.AspNetCore.Http;
using ModEndpoints.Core;
using ModResults;

namespace ModEndpoints;

/// <summary>
/// Abstract base class for endpoints that convert a <see cref="Result{TResponse}"/> business result returned from HandleAsync method to an <see cref="IResult"/> HTTP response, depending on the business result type, state and failure type (if any).
/// </summary>
/// <typeparam name="TRequest">Request type.</typeparam>
/// <typeparam name="TResponse">Type of the value contained by business result response.</typeparam>
public abstract class WebResultEndpoint<TRequest, TResponse>
  : BaseWebResultEndpoint<TRequest, WebResult<TResponse>>
  where TRequest : notnull
  where TResponse : notnull
{
  protected override ValueTask<IResult> ConvertResultToResponseAsync(
    WebResult<TResponse> result,
    HttpContext context,
    CancellationToken ct)
  {
    return result.ExecuteAsync(context, ct);
  }

  protected sealed override async ValueTask<IResult> HandleInvalidValidationResultAsync(
    RequestValidationResult validationResult,
    HttpContext context,
    CancellationToken ct)
  {
    return await ConvertResultToResponseAsync(
      await HandleValidationFailureAsync(validationResult, context, ct).ConfigureAwait(false),
      context,
      ct).ConfigureAwait(false);
  }

  /// <summary>
  /// Handles a failed request validation by generating an appropriate web result response.
  /// </summary>
  /// <remarks>Override this method to customize the response returned when request validation fails.</remarks>
  /// <param name="validationResult">The result of the request validation containing details about the validation failure.</param>
  /// <param name="context">The HTTP context for the current request.</param>
  /// <param name="ct">A cancellation token that can be used to cancel the operation.</param>
  /// <returns>A value task that represents the asynchronous operation. The result contains a web result indicating the
  /// validation failure.</returns>
  protected virtual ValueTask<WebResult<TResponse>> HandleValidationFailureAsync(
    RequestValidationResult validationResult,
    HttpContext context,
    CancellationToken ct)
  {
    var invalidResult = validationResult.ToInvalidResult<TResponse>();
    return ValueTask.FromResult(WebResults.FromResult(invalidResult));
  }
}

/// <summary>
/// Abstract base class for endpoints that convert a <see cref="Result"/> business result returned from HandleAsync method to an <see cref="IResult"/> HTTP response, depending on the business result type, state and failure type (if any).
/// </summary>
/// <typeparam name="TRequest">Request type.</typeparam>
public abstract class WebResultEndpoint<TRequest>
  : BaseWebResultEndpoint<TRequest, WebResult>
  where TRequest : notnull
{
  protected override ValueTask<IResult> ConvertResultToResponseAsync(
    WebResult result,
    HttpContext context,
    CancellationToken ct)
  {
    return result.ExecuteAsync(context, ct);
  }

  protected sealed override async ValueTask<IResult> HandleInvalidValidationResultAsync(
    RequestValidationResult validationResult,
    HttpContext context,
    CancellationToken ct)
  {
    return await ConvertResultToResponseAsync(
      await HandleValidationFailureAsync(validationResult, context, ct).ConfigureAwait(false),
      context,
      ct).ConfigureAwait(false);
  }

  /// <summary>
  /// Handles a failed request validation by generating an appropriate web result response.
  /// </summary>
  /// <remarks>Override this method to customize the response returned when request validation fails.</remarks>
  /// <param name="validationResult">The result of the request validation containing details about the validation failure.</param>
  /// <param name="context">The HTTP context for the current request.</param>
  /// <param name="ct">A cancellation token that can be used to cancel the operation.</param>
  /// <returns>A value task that represents the asynchronous operation. The result contains a web result indicating the
  /// validation failure.</returns>
  protected virtual ValueTask<WebResult> HandleValidationFailureAsync(
    RequestValidationResult validationResult,
    HttpContext context,
    CancellationToken ct)
  {
    var invalidResult = validationResult.ToInvalidResult();
    return ValueTask.FromResult(WebResults.FromResult(invalidResult));
  }
}
